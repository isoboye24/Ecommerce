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
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private AppDBContext db;
        public OrderHeaderRepository(AppDBContext _db) : base(_db)
        {
            db = _db;
        }

        public void Update(OrderHeader obj)
        {
            db.OrderHeaders.Update(obj);
        }
    }
}
