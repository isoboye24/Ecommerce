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
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private AppDBContext db;
        public ShoppingCartRepository(AppDBContext _db) : base(_db)
        {
            db = _db;
        }
        public void Update(ShoppingCart obj)
        {
            db.ShoppingCarts.Update(obj);
        }
    }
}
