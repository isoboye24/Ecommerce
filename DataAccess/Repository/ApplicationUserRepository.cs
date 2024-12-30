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
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private AppDBContext db;
        public ApplicationUserRepository(AppDBContext _db) : base(_db)
        {
            db = _db;
        }

        public void Update(ApplicationUser obj)
        {
            db.ApplicationUsers.Update(obj);
        }
    }
}
