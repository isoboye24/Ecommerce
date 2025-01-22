using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private AppDBContext db;
        public ProductRepository(AppDBContext _db) : base(_db)
        {
            db = _db;
        }

        public void Update(Product obj)
        {
            var objFromDb = db.Products.FirstOrDefault(x => x.ProductID == obj.ProductID);
            if (objFromDb != null)
            {
                objFromDb.Title = obj.Title;
                objFromDb.ISBN = obj.ISBN;
                objFromDb.Price = obj.Price;
                objFromDb.Price50 = obj.Price50;
                objFromDb.Price100 = obj.Price100;
                objFromDb.Description = obj.Description;
                objFromDb.CategoryID = obj.CategoryID;
                objFromDb.Category = obj.Category;
                objFromDb.Author = obj.Author;
                //if (objFromDb.ImageUrl != null)
                //{
                //    objFromDb.ImageUrl = obj.ImageUrl;
                //}
            }
        }
    }
}
