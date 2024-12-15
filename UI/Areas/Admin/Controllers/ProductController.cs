using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Models.ViewModels;

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
            return View(products);
        }
        public IActionResult Upsert(int? id)
        {            
            ProductViewModels productVM = new()
            {
                CategoryList = unitOfWork.Category.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.ID.ToString()
                }),
                Product = new Product()
            };
            if (id == null || id == 0)
            {
                // create
                return View(productVM);
            }
            else
            {
                // update
                productVM.Product = unitOfWork.Product.Get(x => x.ID == id);
                return View(productVM);
            }
        }
        [HttpPost]
        public IActionResult Upsert(ProductViewModels productVM, IFormFile file) 
        {            
            if (ModelState.IsValid)
            {
                unitOfWork.Product.Add(productVM.Product);
                unitOfWork.Save();
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = unitOfWork.Category.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.ID.ToString()
                });
                return View(productVM);
            }
            
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
