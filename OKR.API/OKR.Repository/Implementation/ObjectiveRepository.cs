using Maynghien.Infrastructure.Repository;
using NetTopologySuite.IO;
using OKR.Infrastructure.Enum;
using OKR.Models.Context;
using OKR.Models.Entity;
using OKR.Repository.Contract;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKR.Repository.Implementation
{
    public class ObjectiveRepository : GenericRepository<Objective, OKRDBContext, ApplicationUser>, IObjectiveRepository
    {
        public ObjectiveRepository(OKRDBContext unitOfWork) : base(unitOfWork)
        {
        }

        public void Add(Objective obj, List<KeyResults> keyResults, List<Sidequests> sidequests)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    obj.CreatedOn = DateTime.UtcNow;
                    _context.Add(obj);

                    foreach (var item in keyResults)
                    {
                        item.CreatedOn = DateTime.UtcNow;
                        item.ObjectiveId = obj.Id;
                        item.Active = true;
                    }
                    _context.AddRange(keyResults);

                    foreach (var item in sidequests)
                    {
                        item.CreatedOn = DateTime.UtcNow;
                    }
                    _context.AddRange(sidequests);
                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
            }
        } 

        public Dictionary<Guid,int> caculatePercentObjective(IQueryable<Objective> input)
        {
            var result = input.Select(obj => new
            {
                pointObj = _context.KeyResults.Where(kr => kr.ObjectiveId == obj.Id)
                .Select(kr => new
                {
                    krPoint = kr.Unit != TypeUnitKeyResult.Checked ? _context.Sidequests.Where(sq => sq.KeyResultsId == kr.Id).Count() == 0
                                                    ? (kr.CurrentPoint/ (double)kr.MaximunPoint)
                    :(kr.CurrentPoint / kr.MaximunPoint) / 2
                    + (_context.Sidequests.Where(sq => sq.KeyResultsId == kr.Id && sq.Status == true).Count() /
                        (double)_context.Sidequests.Where(sq => sq.KeyResultsId == kr.Id).Count()) / 2
                    : (_context.Sidequests.Where(sq => sq.KeyResultsId == kr.Id && sq.Status == true).Count() /
                        (double)_context.Sidequests.Where(sq => sq.KeyResultsId == kr.Id).Count()) ,
                    krId = kr.Id,
                }).ToList(),
                    Id = obj.Id
                }).ToList();

            // Convert the result to a dictionary with the correct calculations
            var resultDictionary = result.ToDictionary(
                x => x.Id,
                x => (int)x.pointObj.Average(po => po.krPoint *100)
            );

            return resultDictionary;
        }
        public int caculateOveralProgress(IQueryable<Objective> input)
        {
            var result = input.Select(obj => new
            {
                pointObj = _context.KeyResults.Where(kr => kr.ObjectiveId == obj.Id)
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
            var point = result.ToDictionary(
                x => x.Id,
                x => (int)x.pointObj.Average(po => po.krPoint * 100)
            ).Average(x=>x.Value);

            return (int)point;
        }
    }
}
