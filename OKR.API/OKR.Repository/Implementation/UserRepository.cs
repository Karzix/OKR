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
    public class UserRepository : IUserRepository
    {
        private OKRDBContext _context;


        public UserRepository(OKRDBContext context)
        {
            _context = context;
        }

        public IQueryable FindBy(Expression<Func<ApplicationUser, bool>> predicate)
        {
            return _context.Users.Where(predicate);
        }
        public int CountRecordsByPredicate(Expression<Func<ApplicationUser, bool>> predicate)
        {
            return _context.Users.Where(predicate).Count();
        }
        public IQueryable<ApplicationUser> FindByPredicate(Expression<Func<ApplicationUser, bool>> predicate)
        {
            return _context.Users.Where(predicate).AsQueryable();
        }

        public IQueryable<ApplicationUser> AsQueryable()
        {
            var query = _context.Users.AsQueryable();
            return query;
        }
    }
}
