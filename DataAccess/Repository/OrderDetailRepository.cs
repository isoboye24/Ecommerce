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
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private AppDBContext db;
        public OrderDetailRepository(AppDBContext _db) : base(_db)
        {
            db = _db;
        }

        public void Update(OrderDetail obj)
        {
            db.OrderDetails.Update(obj);
        }
    }
}
