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

        public void UpdateStatus(int ID, string orderStatus, string? paymentStatus = null)
        {
            var orderFromDB = db.OrderHeaders.FirstOrDefault(x => x.OrderHeaderID == ID);
            if (orderFromDB != null)
            {
                orderFromDB.OrderStatus = orderStatus;
                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    orderFromDB.PaymentStatus = paymentStatus;
                }
            }
        }

        public void UpdateStripePaymentID(int ID, string sessionID, string paymentIntentID)
        {
            var orderFromDB = db.OrderHeaders.FirstOrDefault(x => x.OrderHeaderID == ID);
            if (!string.IsNullOrEmpty(sessionID))
            {
                orderFromDB.SessionID = sessionID;
            }
            if (!string.IsNullOrEmpty(paymentIntentID))
            {
                orderFromDB.PaymentIntentID = paymentIntentID;
                orderFromDB.PaymentDate = DateTime.Now;
            }
        }
    }
}
