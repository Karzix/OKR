using Maynghien.Infrastructure.Repository;
using OKR.Models.Context;
using OKR.Models.Entity;

namespace OKR.Repository.Contract
{
    public interface IDepartmentObjectivesRepository : IGenericRepository<DepartmentObjectives, OKRDBContext, ApplicationUser>
    {
    }
}
