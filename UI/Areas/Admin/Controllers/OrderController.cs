using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ViewModels;
using System.Diagnostics;
using System.Security.Claims;
using Utility;

namespace UI.Areas.Admin.Controllers
{
    [Area("admin")]
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
