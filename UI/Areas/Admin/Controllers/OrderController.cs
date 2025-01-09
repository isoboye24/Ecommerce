using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace UI.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public OrderController(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<OrderHeader> orderHeaderList = unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
            return Json(new { data = orderHeaderList });
        }
        #endregion
    }
}
