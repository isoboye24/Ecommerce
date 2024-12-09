using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace UI.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository categoryRepo;
        public CategoryController(ICategoryRepository _db)
        {
            categoryRepo = _db;
        }
        public IActionResult Index()
        {
            List<Category> categories = categoryRepo.GetAll().ToList();
            return View(categories);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The DisplayOrder cannot exactly match the Name.");
            }            
            if (ModelState.IsValid)
            {
                categoryRepo.Add(obj);
                categoryRepo.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category categoryItem = categoryRepo.Get(x=>x.ID == id);
            if (categoryItem == null)
            {
                return NotFound();
            }
            return View(categoryItem);
        }
        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                categoryRepo.Update(obj);
                categoryRepo.Save();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category categoryItem = categoryRepo.Get(x=>x.ID == id);
            if (categoryItem == null)
            {
                return NotFound();
            }
            return View(categoryItem);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Category? categoryItem = categoryRepo.Get(x => x.ID == id);
            if (categoryItem == null)
            {
                return NotFound();
            }
            categoryRepo.Remove(categoryItem);
            categoryRepo.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
