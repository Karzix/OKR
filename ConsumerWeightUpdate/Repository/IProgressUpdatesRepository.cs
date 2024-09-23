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
    public interface IProgressUpdatesRepository : IGenericRepository<ProgressUpdates,OKRDBContext,ApplicationUser>
    {
    }
}
