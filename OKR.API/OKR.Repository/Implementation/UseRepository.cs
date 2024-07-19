using OKR.Models.Context;
using OKR.Models.Entity;
using OKR.Repository.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OKR.Repository.Implementation
{
    public class UseRepository : IUseRepository
    {
        private OKRDBContext _context;


        public UseRepository(OKRDBContext context)
        {
            _context = context;
        }

        public IQueryable FindBy(Expression<Func<ApplicationUser, bool>> predicate)
        {
            return _context.Users.Where(predicate);
        }
    }
}
