﻿using Maynghien.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using OKR.Infrastructure.Enum;
using OKR.Models.Context;
using OKR.Models.Entity;
using OKR.Repository.Contract;
using static OKR.Infrastructure.Enum.helperQuarter;

namespace OKR.Repository.Implementation
{
    public class ObjectivesRepository : GenericRepository<Objectives, OKRDBContext, ApplicationUser>, IObjectivesRepository
    {
        public ObjectivesRepository(OKRDBContext unitOfWork) : base(unitOfWork)
        {
        }

        public void Add(Objectives obj, List<KeyResults> keyResults)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    obj.CreatedOn = DateTime.UtcNow;
                    _context.Objectives.Add(obj);

                    keyResults.ForEach(keyResults =>
                    {
                        keyResults.CreatedOn = DateTime.UtcNow;
                        keyResults.CreatedBy = obj.CreatedBy;
                    });
                    _context.KeyResults.AddRange(keyResults);
                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public int caculateOveralProgress(IQueryable<Objectives> input)
        {
            var result = input.Select(obj =>
                   _context.KeyResults.Where(kr => kr.ObjectivesId == obj.Id)
                       .Sum(kr => (float)(kr.Percentage / 100.0) * (kr.CurrentPoint / (float)kr.MaximunPoint))
               ).Sum();

            return (int)result;
        }

        public Dictionary<Guid, int> caculatePercentObjectives(IQueryable<Objectives> input)
        {
            var result = input.Select(obj => new
            {
                pointObj = _context.KeyResults.Where(kr => kr.ObjectivesId == obj.Id)
                    .Sum(kr => (float)(kr.Percentage / 100.0) * (kr.CurrentPoint / (float)kr.MaximunPoint)),
                Id = obj.Id
            }).ToDictionary(
                x => x.Id,
                x => (int)x.pointObj 
            );

            return result;
        }

        public void Edit(Objectives updatedObj, List<KeyResults> updatedKeyResults)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Objectives.Update(updatedObj);
                    _context.KeyResults.UpdateRange(updatedKeyResults);
                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }


    }
}
