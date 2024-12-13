using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;

namespace UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public ProductController(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        public IActionResult Index()
        {
            List<Product> products = unitOfWork.Product.GetAll().ToList();
            IEnumerable<SelectListItem> CategoryList = unitOfWork.Category.GetAll().Select(x=> new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.Name
            });
            return View(products);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Product product) 
        {
            if (product.Title == product.ListPrice.ToString())
            {
                ModelState.AddModelError("name", "The Product price list cannot exactly match the Title.");
            }
            if (ModelState.IsValid)
            {
                unitOfWork.Product.Add(product);
                unitOfWork.Save();
                TempData["success"] = "Product created successfully";
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
            Product productItem = unitOfWork.Product.Get(x => x.ID == id);
            if (productItem == null)
            {
                return NotFound();
            }
            return View(productItem);
        }
        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Product.Update(product);
                unitOfWork.Save();
                TempData["success"] = "Product updated successfully";
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
            Product productItem = unitOfWork.Product.Get(x => x.ID == id);
            if (productItem == null)
            {
                return NotFound();
            }
            return View(productItem);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Product? productItem = unitOfWork.Product.Get(x => x.ID == id);
            if (productItem == null)
            {
                return NotFound();
            }
            unitOfWork.Product.Remove(productItem);
            unitOfWork.Save();
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
