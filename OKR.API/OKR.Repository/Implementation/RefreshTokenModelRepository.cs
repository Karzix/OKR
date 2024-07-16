using Maynghien.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using OKR.Models;
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
    public class RefreshTokenModelRepository : GenericRepository<RefreshTokenModel, OKRDBContext, ApplicationUser>, IRefreshTokenModelRepository
    {
        public RefreshTokenModelRepository(OKRDBContext unitOfWork) : base(unitOfWork)
        {
        }
    }
}
