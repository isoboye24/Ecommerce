using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ViewModels;
using Stripe;
using Stripe.Checkout;
using System.Diagnostics;
using System.Security.Claims;
using Utility;

namespace UI.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        [BindProperty]
        public OrderViewModel OrderVM { get; set; }
        public OrderController(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int orderId)
        {
            OrderVM = new()
            {
                OrderHeader = unitOfWork.OrderHeader.Get(x=>x.OrderHeaderID == orderId, includeProperties: "ApplicationUser"), 
                OrderDetail = unitOfWork.OrderDetail.GetAll(x=>x.OrderHeaderID == orderId, includeProperties: "Product") 
            };
            return View(OrderVM);
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult UpdateOrderDetail()
        {
            var orderHeaderFromDB = unitOfWork.OrderHeader.Get(x => x.OrderHeaderID == OrderVM.OrderHeader.OrderHeaderID);
            orderHeaderFromDB.Name = OrderVM.OrderHeader.Name;
            orderHeaderFromDB.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDB.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeaderFromDB.City = OrderVM.OrderHeader.City;
            orderHeaderFromDB.State = OrderVM.OrderHeader.State;
            orderHeaderFromDB.PostalCode = OrderVM.OrderHeader.PostalCode;
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
            {
                orderHeaderFromDB.Carrier = OrderVM.OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
            {
                orderHeaderFromDB.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            }
            unitOfWork.OrderHeader.Update(orderHeaderFromDB);
            unitOfWork.Save();

            TempData["Success"] = "Order Details Updated Successfully.";
            return RedirectToAction(nameof(Details), new {orderId = orderHeaderFromDB.OrderHeaderID});
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult StartProcessing()
        {
            unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.OrderHeaderID, SD.StatusInProcess);
            unitOfWork.Save();
            TempData["Success"] = "Order Details Updated Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.OrderHeaderID });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder()
        {
            var orderHeader = unitOfWork.OrderHeader.Get(x => x.OrderHeaderID == OrderVM.OrderHeader.OrderHeaderID);
            orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
            orderHeader.OrderStatus = SD.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                orderHeader.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            }
            
            unitOfWork.OrderHeader.Update(orderHeader);
            unitOfWork.Save();
            TempData["Success"] = "Order Shipped Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.OrderHeaderID });
        }
        
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancelOrder()
        {
            var orderHeader = unitOfWork.OrderHeader.Get(x => x.OrderHeaderID == OrderVM.OrderHeader.OrderHeaderID);
            
            if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentID
                };
                var service = new RefundService();
                Refund refund = service.Create(options);
                unitOfWork.OrderHeader.UpdateStatus(orderHeader.OrderHeaderID, SD.StatusCancelled, SD.StatusRefunded);
            }
            else
            {
                unitOfWork.OrderHeader.UpdateStatus(orderHeader.OrderHeaderID, SD.StatusCancelled, SD.StatusCancelled);
            }
            unitOfWork.Save();
            TempData["Success"] = "Order Cancelled Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.OrderHeaderID });
        }

        [ActionName("Details")]
        [HttpPost]
        public IActionResult Details_Pay_Now()
        {
            OrderVM.OrderHeader = unitOfWork.OrderHeader.Get(x => x.OrderHeaderID == OrderVM.OrderHeader.OrderHeaderID, includeProperties: "ApplicationUser");
            OrderVM.OrderDetail = unitOfWork.OrderDetail.GetAll(x => x.OrderHeaderID == OrderVM.OrderHeader.OrderHeaderID, includeProperties: "Product");

            // Stripe logic
            var domain = "https://localhost:7067/";
            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderId={OrderVM.OrderHeader.OrderHeaderID}",
                CancelUrl = domain + $"admin/order/details?orderId={OrderVM.OrderHeader.OrderHeaderID}",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var item in OrderVM.OrderDetail)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100), // 20.50 = 2050
                        Currency = "gbp",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title
                        }
                    },
                    Quantity = item.Count
                };
                options.LineItems.Add(sessionLineItem);
            }
            var service = new SessionService();
            Session session = service.Create(options);
            unitOfWork.OrderHeader.UpdateStripePaymentID(OrderVM.OrderHeader.OrderHeaderID, session.Id, session.PaymentIntentId);
            unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);            
        }

        public IActionResult PaymentConfirmation(int orderHeaderId)
        {
            OrderHeader orderHeader = unitOfWork.OrderHeader.Get(x => x.OrderHeaderID == orderHeaderId);
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                // This is an order by company
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionID);
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    unitOfWork.OrderHeader.UpdateStripePaymentID(orderHeaderId, session.Id, session.PaymentIntentId);
                    unitOfWork.OrderHeader.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                    unitOfWork.Save();
                }
            }
            
            return View(orderHeaderId);
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> orderHeaderList;

            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                orderHeaderList = unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
            }
            else
            {
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                orderHeaderList = unitOfWork.OrderHeader.GetAll(x => x.ApplicationUserId == userId, includeProperties: "ApplicationUser");
            }

            switch (status)
            {
                case "inprocess":
                    orderHeaderList = orderHeaderList.Where(x=>x.OrderStatus == SD.StatusInProcess);
                    break;
                case "pending":
                    orderHeaderList = orderHeaderList.Where(x => x.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "completed":
                    orderHeaderList = orderHeaderList.Where(x => x.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    orderHeaderList = orderHeaderList.Where(x => x.OrderStatus == SD.StatusApproved);
                    break;
                default:                    
                    break;
            }
            return Json(new { data = orderHeaderList });
        }
        #endregion
    }
}
