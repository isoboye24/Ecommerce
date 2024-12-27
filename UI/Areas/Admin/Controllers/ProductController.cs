using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Models.ViewModels;
using Utility;

namespace UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IWebHostEnvironment webHostEnvironment;
        public ProductController(IUnitOfWork _unitOfWork, IWebHostEnvironment _webHostEnvironment)
        {
            unitOfWork = _unitOfWork;
            webHostEnvironment = _webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> productList = unitOfWork.Product.GetAll(includeProperties:"Category").ToList();            
            return View(productList);
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
        public IActionResult Upsert(ProductViewModels productVM, IFormFile? file) 
        {            
            if (ModelState.IsValid)
            {
                string wwwRootPath = webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        // delete the old image
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, filename), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"\images\product\" + filename;
                }
                if (productVM.Product.ID == 0)
                {
                    unitOfWork.Product.Add(productVM.Product);
                    TempData["success"] = "Product created successfully";
                }
                else
                {
                    unitOfWork.Product.Update(productVM.Product);
                    TempData["success"] = "Product updated successfully";
                }
                unitOfWork.Save();                
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
        
        #region API CALLS
        public IActionResult GetAll()
        {
            List<Product> productList = unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new {data = productList});
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = unitOfWork.Product.Get(x=>x.ID == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting. No item was found!" });
            }
            else
            {
                if (productToBeDeleted.ImageUrl != null)
                {
                    var oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }                   
                unitOfWork.Product.Remove(productToBeDeleted);
                unitOfWork.Save();
                return Json(new { success = true, message = "Product deleted successfully!" });                               
            }
        }
        #endregion
    }
}
