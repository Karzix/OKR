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

    }
}
