using DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace UI.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDBContext db;
        public CategoryController(AppDBContext _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {
            List<Category> categories = db.Categories.ToList();
            return View(categories);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            //if (obj.Name == obj.DisplayOrder.ToString()) 
            //{
            //    ModelState.AddModelError("name", "The DisplayOrder cannot exactly match the Name.");
            //}
            //if (obj.Name != null && obj.Name.ToLower() == "test") 
            //{
            //    ModelState.AddModelError("", "Test is an invalid value.");
            //}
            if (ModelState.IsValid)
            {
                db.Categories.Add(obj);
                db.SaveChanges();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
