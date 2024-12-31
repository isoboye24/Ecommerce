﻿using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader obj);
        void UpdateStatus(int ID, string orderStatus, string? paymentStatus = null);
        void UpdateStripePaymentID(int ID, string sessionID, string paymentIntentID);
    }
}
