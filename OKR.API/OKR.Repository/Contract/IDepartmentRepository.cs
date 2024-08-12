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
    public interface IDepartmentRepository : IGenericRepository<Department, OKRDBContext, ApplicationUser>
    {
        Department GetParentOfChildDepartment(int levelParent, Guid idChild);
        List<Department> GetAllChildDepartments(Guid parentId);
    }
}
