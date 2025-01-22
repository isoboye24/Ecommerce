using DataAccess.Data;
using DataAccess.Repository.IRepository;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.ViewModels;
using Utility;

namespace UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly AppDBContext db;
        private readonly UserManager<IdentityUser> _userManager;
        public UserController(AppDBContext _db, UserManager<IdentityUser> userManager)
        {
            db = _db;
            _userManager = userManager;
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

        [HttpPost]
        public IActionResult LockUnlock([FromBody]string id)
        {
            var objFromDB = db.ApplicationUsers.FirstOrDefault(x=>x.Id == id);
            if (objFromDB == null)
            {
                return Json(new { success = false, message = "Error while locking / unlocking" });
            }
            else
            {
                if (objFromDB.LockoutEnd != null && objFromDB.LockoutEnd > DateTime.Now)
                {
                    objFromDB.LockoutEnd = DateTime.Now;
                }
                else
                {
                    objFromDB.LockoutEnd = DateTime.Now.AddYears(1000);
                }
            }
            db.SaveChanges();
            return Json(new { success = true, message = "Operation Successful" });
        }
        
        public IActionResult RoleManagement(string userId)
        {
            string RoleID = db.UserRoles.FirstOrDefault(x=>x.UserId == userId).RoleId;
            RoleManagementVM RoleMV = new RoleManagementVM()
            {
                ApplicationUser = db.ApplicationUsers.Include(x=>x.Company).FirstOrDefault(x=>x.Id== userId),
                RoleList = db.Roles.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Name
                }),
                CompanyList = db.Companies.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.CompanyID.ToString()
                }),
            };
            RoleMV.ApplicationUser.Role = db.Roles.FirstOrDefault(x => x.Id == RoleID).Name;
            return View(RoleMV);
        }
        [HttpPost]
        public IActionResult RoleManagement(RoleManagementVM roleManagementVM)
        {
            string RoleID = db.UserRoles.FirstOrDefault(x => x.UserId == roleManagementVM.ApplicationUser.Id).RoleId;
            string oldRole = db.Roles.FirstOrDefault(x => x.Id == RoleID).Name;

            if (!(roleManagementVM.ApplicationUser.Role == oldRole))
            {
                ApplicationUser applicationUser = db.ApplicationUsers.FirstOrDefault(x=>x.Id == roleManagementVM.ApplicationUser.Id);
                if (roleManagementVM.ApplicationUser.Role == SD.Role_Company)
                {
                    applicationUser.CompanyID = roleManagementVM.ApplicationUser.CompanyID;
                }
                if (oldRole == SD.Role_Company)
                {
                    applicationUser.CompanyID = null;
                }
                db.SaveChanges();
                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, roleManagementVM.ApplicationUser.Role).GetAwaiter().GetResult();
            }
            return RedirectToAction("Index");
        }

        #endregion
    }
}
