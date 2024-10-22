using Maynghien.Infrastructure.Repository;
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
    public class DepartmentProgressApprovalRepository : GenericRepository<DepartmentProgressApproval, OKRDBContext, ApplicationUser>, IDepartmentProgressApprovalRepository
    {
        public DepartmentProgressApprovalRepository(OKRDBContext unitOfWork) : base(unitOfWork)
        {
            _context = unitOfWork;
        }
    }
}
