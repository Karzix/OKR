using MayNghien.Models.Response.Base;
using OKR.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OKR.Repository.Contract
{
    public interface IUserRepository 
    {
        public IQueryable FindBy(Expression<Func<ApplicationUser, bool>> predicate);
        int CountRecordsByPredicate(Expression<Func<ApplicationUser, bool>> predicate);
        public IQueryable<ApplicationUser> FindByPredicate(Expression<Func<ApplicationUser, bool>> predicate);
        public IQueryable<ApplicationUser> AsQueryable();
    }
}
