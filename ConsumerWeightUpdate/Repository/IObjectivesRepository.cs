using Maynghien.Infrastructure.Repository;
using OKR.Models.Context;
using OKR.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsumerWeightUpdate.Repository
{
    public interface IObjectivesRepository : IGenericRepository<Objectives,OKRDBContext, ApplicationUser>
    {
        void Add(Objectives obj, List<KeyResults> keyResults, List<Sidequests> sidequests, Guid? DepartmentId);
        Dictionary<Guid, int> caculatePercentObjectives(IQueryable<Objectives> input);
        int caculateOveralProgress(IQueryable<Objectives> input);
        void Edit(Objectives obj, List<KeyResults> keyResults, List<Sidequests> sidequests);
    }
}
