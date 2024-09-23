using Maynghien.Infrastructure.Repository;
using OKR.Models.Context;
using OKR.Models.Entity;

namespace ConsumerWeightUpdate.Repository
{
    public class KeyResultRepository : GenericRepository<KeyResults, OKRDBContext, ApplicationUser>, IKeyResultRepository
    {
        public KeyResultRepository(OKRDBContext unitOfWork) : base(unitOfWork)
        {
            _context = unitOfWork;
        }
    }
}
