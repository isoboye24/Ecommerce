using DataAccess.Data;
using DataAccess.Repository.IRepository;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Utility;

namespace UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly AppDBContext db;
        public UserController(AppDBContext _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {           
            return View();
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> objUserList = db.ApplicationUsers.Include(x=>x.Company).ToList();
            
            var userRoles = db.UserRoles.ToList();
            var roles = db.Roles.ToList();
            
            foreach (var user in objUserList)
            {
                var roleID = userRoles.FirstOrDefault(x=>x.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(x=>x.Id == roleID).Name;

                if (user.Company == null)
                {
                    user.Company = new Company() { Name = ""};
                }
            } 
            return Json(new { data = objUserList });
        }


        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            
            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}
