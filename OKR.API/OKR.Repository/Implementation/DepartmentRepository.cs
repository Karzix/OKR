using Maynghien.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using OKR.Models.Context;
using OKR.Models.Entity;
using OKR.Repository.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKR.Repository.Implementation
{
    public class DepartmentRepository : GenericRepository<Department, OKRDBContext, ApplicationUser>, IDepartmentRepository
    {
        public DepartmentRepository(OKRDBContext unitOfWork) : base(unitOfWork)
        {
        }

        public Department GetParentOfChildDepartment(int levelParent, Guid idChild)
        {
            var query = _context.Department.Where(x => x.Id == idChild);
            var childDepartment = query.FirstOrDefault();
            if(childDepartment == null)
            {
                return null;
            }
            for (int i = levelParent; i < childDepartment.Level; i++)
            {
                query = query.Join(_context.Department,
                                    clD => clD.ParentDepartmentId,
                                    prD => prD.Id,
                                    (clD, prD) => prD);
            }

            var department = query.FirstOrDefault();
            return department;
        }
        public List<Department> GetAllChildDepartments(Guid parentId)
        {
            var departments = new List<Department>();
            var pendingParents = new List<Guid> { parentId };

            while (pendingParents.Any())
            {
                // Tìm tất cả các department có ParentDepartmentId trong danh sách pendingParents
                var childDepartments = _context.Department
                    .Where(d => pendingParents.Contains(d.ParentDepartmentId.Value))
                    .ToList();

                if (childDepartments.Any())
                {
                    departments.AddRange(childDepartments);
                    pendingParents = childDepartments.Select(d => d.Id).ToList();
                }
                else
                {
                    break;
                }
            }

            return departments;
        }
    }
}
