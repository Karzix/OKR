using Maynghien.Infrastructure.Repository;
using OKR.Infrastructure.Enum;
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
    public class UserObjectivesRepository : GenericRepository<UserObjectives, OKRDBContext, ApplicationUser>, IUserObjectivesRepository
    {
        public UserObjectivesRepository(OKRDBContext unitOfWork) : base(unitOfWork)
        {
        }
        public Dictionary<Guid, int> caculatePercentObjectives(IQueryable<UserObjectives> input)
        {
            var result = input.Select(obj => new
            {
                pointObj = _context.KeyResults.Where(kr => kr.ObjectivesId == obj.ObjectivesId)
                .Select(kr => new
                {
                    krPoint = kr.Unit != TypeUnitKeyResult.Checked ? _context.Sidequests.Where(sq => sq.KeyResultsId == kr.Id).Count() == 0
                                                    ? (kr.CurrentPoint / (double)kr.MaximunPoint)
                    : (kr.CurrentPoint / kr.MaximunPoint) / 2
                    + (_context.Sidequests.Where(sq => sq.KeyResultsId == kr.Id && sq.Status == true).Count() /
                        (double)_context.Sidequests.Where(sq => sq.KeyResultsId == kr.Id).Count()) / 2
                    : (_context.Sidequests.Where(sq => sq.KeyResultsId == kr.Id && sq.Status == true).Count() /
                        (double)_context.Sidequests.Where(sq => sq.KeyResultsId == kr.Id).Count()),
                    krId = kr.Id,
                }).ToList(),
                Id = obj.Id
            }).ToList();

            // Convert the result to a dictionary with the correct calculations
            var resultDictionary = result.ToDictionary(
                x => x.Id,
                x => (int)x.pointObj.Average(po => po.krPoint * 100)
            );

            return resultDictionary;
        }
    }
}
