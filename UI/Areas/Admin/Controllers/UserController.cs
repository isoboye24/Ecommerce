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
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
            _roleManager = roleManager;
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
            List<ApplicationUser> objUserList = unitOfWork.ApplicationUser.GetAll(includeProperties: "Company").ToList();

            foreach (var user in objUserList)
            {
                user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();
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
            var objFromDB = unitOfWork.ApplicationUser.Get(x=>x.Id == id);
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
            unitOfWork.ApplicationUser.Update(objFromDB);
            unitOfWork.Save();
            return Json(new { success = true, message = "Operation Successful" });
        }
        
        public IActionResult RoleManagement(string userId)
        {
            RoleManagementVM RoleMV = new RoleManagementVM()
            {
                ApplicationUser = unitOfWork.ApplicationUser.Get(x=>x.Id == userId, includeProperties: "Company"),
                RoleList = _roleManager.Roles.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Name
                }),
                CompanyList = unitOfWork.Company.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.CompanyID.ToString()
                }),
            };
            RoleMV.ApplicationUser.Role = _userManager.GetRolesAsync(unitOfWork.ApplicationUser.Get(x=>x.Id == userId)).GetAwaiter().GetResult().FirstOrDefault();
            return View(RoleMV);
        }
        [HttpPost]
        public IActionResult RoleManagement(RoleManagementVM roleManagementVM)
        {
            string oldRole = _userManager.GetRolesAsync(unitOfWork.ApplicationUser.Get(x => x.Id == roleManagementVM.ApplicationUser.Id)).GetAwaiter().GetResult().FirstOrDefault();

            ApplicationUser applicationUser = unitOfWork.ApplicationUser.Get(x => x.Id == roleManagementVM.ApplicationUser.Id);

            if (!(roleManagementVM.ApplicationUser.Role == oldRole))
            {                
                if (roleManagementVM.ApplicationUser.Role == SD.Role_Company)
                {
                    applicationUser.CompanyID = roleManagementVM.ApplicationUser.CompanyID;
                }
                if (oldRole == SD.Role_Company)
                {
                    applicationUser.CompanyID = null;
                }
                unitOfWork.ApplicationUser.Update(applicationUser);
                unitOfWork.Save();

                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, roleManagementVM.ApplicationUser.Role).GetAwaiter().GetResult();
            }
            else
            {
                if (oldRole == SD.Role_Company && applicationUser.CompanyID != roleManagementVM.ApplicationUser.CompanyID)
                {
                    applicationUser.CompanyID = roleManagementVM.ApplicationUser.CompanyID;
                    unitOfWork.ApplicationUser.Update(applicationUser);
                    unitOfWork.Save();
                }
            }
            return RedirectToAction("Index");
        }

        #endregion
    }
}
