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
    public interface IUseRepository 
    {
        public IQueryable FindBy(Expression<Func<ApplicationUser, bool>> predicate);
    }
}
