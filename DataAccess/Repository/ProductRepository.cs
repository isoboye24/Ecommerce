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
            db.Products.Update(obj);
        }
    }
}
