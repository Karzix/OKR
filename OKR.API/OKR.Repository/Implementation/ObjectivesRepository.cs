using Maynghien.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
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
    public class ObjectivesRepository : GenericRepository<Objectives, OKRDBContext, ApplicationUser>, IObjectivesRepository
    {
        public ObjectivesRepository(OKRDBContext unitOfWork) : base(unitOfWork)
        {
        }

        public void Add(Objectives obj, List<KeyResults> keyResults, List<Sidequests> sidequests, Guid? DepartmentId)
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
                        item.ObjectivesId = obj.Id;
                        item.Active = true;
                    }
                    _context.AddRange(keyResults);

                    foreach (var item in sidequests)
                    {
                        item.CreatedOn = DateTime.UtcNow;
                    }
                    _context.AddRange(sidequests);

                    if(obj.TargetType == TargetType.individual)
                    {
                        var newUserObj = new UserObjectives
                        {
                            ApplicationUserId = _context.Users.Where(x => x.UserName == obj.CreatedBy).First().Id,
                            CreatedBy = obj.CreatedBy,
                            Id = Guid.NewGuid(),
                            ObjectivesId = obj.Id,
                            CreatedOn = DateTime.UtcNow,
                        };
                        _context.UserObjectives.Add(newUserObj);
                    }
                    else
                    {
                        var newDepartmentObj = new DepartmentObjectives
                        {
                            CreatedBy = obj.CreatedBy,
                            DepartmentId = DepartmentId.Value,
                            CreatedOn = DateTime.UtcNow,
                            ObjectivesId = obj.Id,
                            Id = Guid.NewGuid(),
                        };
                        _context.DepartmentObjectives.Add(newDepartmentObj);
                    }

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

        public Dictionary<Guid,int> caculatePercentObjectives(IQueryable<Objectives> input)
        {
            var result = input.Select(obj => new
            {
                pointObj = _context.KeyResults.Where(kr => kr.ObjectivesId == obj.Id)
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
                x => (int)x.pointObj.Average(po => po.krPoint * 100)
            );

            return resultDictionary;
        }
        public int caculateOveralProgress(IQueryable<Objectives> input)
        {
            var result = input.Select(obj => new
            {
                pointObj = _context.KeyResults.Where(kr => kr.ObjectivesId == obj.Id)
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

        public void Edit(Objectives obj, List<KeyResults> keyResults, List<Sidequests> sidequests)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Kiểm tra sự tồn tại của đối tượng
                    if (_context.Objectives.Any(o => o.Id == obj.Id) == false)
                    {
                        throw new Exception("Objective does not exist.");
                    }

                    _context.Objectives.Update(obj);

                    var existingKeyResults = _context.KeyResults.Where(kr => kr.ObjectivesId == obj.Id).ToList();

                    var listKeyResultsEdit = keyResults.Where(x => existingKeyResults.Any(kr => kr.Id == x.Id)).ToList();
                    _context.KeyResults.UpdateRange(listKeyResultsEdit);

                    var listNewKeyResults = keyResults.Where(x => existingKeyResults.All(kr => kr.Id != x.Id)).ToList();
                    _context.KeyResults.AddRange(listNewKeyResults);

                    var ListIdKeyresults = listKeyResultsEdit.Select(x => x.Id).ToList();

                    var listSidequestsEdit = sidequests.Where(x => ListIdKeyresults.Contains(x.Id)).ToList();
                    _context.Sidequests.UpdateRange(listSidequestsEdit);

                    var listNewSidequests = sidequests.Where(x => !ListIdKeyresults.Contains(x.Id)).ToList();
                    _context.Sidequests.AddRange(listNewSidequests);

                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch (DbUpdateException dbEx)
                {
                    transaction.Rollback();
                    // Log inner exception details for further investigation
                    Console.WriteLine(dbEx.InnerException?.Message);
                    throw;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // Log general exception details
                    Console.WriteLine(ex.Message);
                    throw;
                }
            }
        }
    }
}
