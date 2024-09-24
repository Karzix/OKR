using Maynghien.Infrastructure.Repository;
using OKR.Infrastructure.Enum;
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
        public int caculatePercentKeyResults(KeyResults input)
        {
            var point = 0.0;
            var keyresults = input;
            var sidequest = _context.Sidequests.Where(x => x.KeyResultsId == keyresults.Id || x.IsDeleted != true);
            if (keyresults.Unit == TypeUnitKeyResult.Checked)
            {
                point = (sidequest.Where(x => x.Status == true).Count() / (double)sidequest.Count());
                return (int)point;
            }

            if (sidequest.Count() > 0)
            {
                point = sidequest.Where(x => x.Status == true).Count() / (double)sidequest.Count();
                point = (point / 2.0) + (keyresults.CurrentPoint / (double)keyresults.MaximunPoint) / 2.0;
            }
            else
            {
                point = (keyresults.CurrentPoint / (double)keyresults.MaximunPoint);
            }
            return (int)(point * 100);
        }
    }
}
