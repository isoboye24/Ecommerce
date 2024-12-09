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
    }
}
