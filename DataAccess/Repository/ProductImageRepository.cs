using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
    {
        private AppDBContext db;
        public ProductImageRepository(AppDBContext _db) : base(_db)
        {
            db = _db;
        }

        public void Update(ProductImage obj)
        {
            db.ProductImages.Update(obj);
        }
    }
}
