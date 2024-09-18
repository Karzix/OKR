using Maynghien.Infrastructure.Repository;
using OKR.Models.Context;
using OKR.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKR.Repository.Contract
{
    public interface IUserObjectivesRepository : IGenericRepository<UserObjectives, OKRDBContext, ApplicationUser>
    {
        Dictionary<Guid, int> caculatePercentObjectives(IQueryable<UserObjectives> input);
    }
}
