﻿using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private AppDBContext db;
        public CompanyRepository(AppDBContext _db) : base(_db)
        {
            db = _db;
        }
        public void Update(Company obj)
        {
            db.Companies.Update(obj);
        }
    }
}
