using System.Diagnostics;
using System.Security.Claims;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Utility;

namespace UI.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork _unitOfWork)
        {
            _logger = logger;
            unitOfWork = _unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = unitOfWork.Product.GetAll(includeProperties: "Category");
            return View(productList);
        }
        public IActionResult Details(int productID)
        {
            ShoppingCart cart = new()
            {
                Product = unitOfWork.Product.Get(x => x.ID == productID, includeProperties: "Category"),
                Count = 1,
                ProductID = productID
            };            
            return View(cart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userID = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userID;

            ShoppingCart cartFromDB = unitOfWork.ShoppingCart.Get(x=>x.ApplicationUserId == userID
            && x.ProductID == shoppingCart.ProductID);
            if (cartFromDB != null)
            {
                // shopping cart exists
                cartFromDB.Count += shoppingCart.Count;
                unitOfWork.ShoppingCart.Update(cartFromDB);
                unitOfWork.Save();
            }
            else
            {
                // add cart record
                unitOfWork.ShoppingCart.Add(shoppingCart);
                unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart, unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userID).Count());
            }
            TempData["success"] = "Cart updated successfully";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
