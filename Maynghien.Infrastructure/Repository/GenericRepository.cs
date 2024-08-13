
using EFCore.BulkExtensions;
using MayNghien.Infrastructure.Models;
using MayNghien.Infrastructure.Models.Entity;
using MayNghien.Infrastructure.Request.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Maynghien.Infrastructure.Repository
{
    public class GenericRepository<TEntity, TContext, TUser> : IGenericRepository<TEntity, TContext, TUser>
        where TEntity : BaseEntity where TContext : BaseContext<TUser> where TUser : IdentityUser
    {
        #region Properties
        public TContext _context;
        private bool disposed = false;

        public TContext DbContext
        {
            get
            {
                return _context;
            }
        }
        #endregion

        #region Constructor
        public GenericRepository(TContext unitOfWork)
        {
            _context = unitOfWork;
        }


        #endregion

        #region Method
        public virtual void Add(TEntity item)
        {
            if (item != null)
            {
                item.CreatedOn = DateTime.UtcNow;
                if (item.CreatedBy == null)
                {
                    item.CreatedBy = "";
                }
                _context.Add(item);
                _context.SaveChanges();
            }
        }


        public void Delete(TEntity entity)
        {
            if (entity != null)
            {
                _context.Attach(entity);
                _context.Remove(entity);
                _context.SaveChanges();
            }
        }

        public void Edit(TEntity entity)
        {
            if (entity != null)
            {
                _context.Update(entity);
                _context.SaveChanges();
            }
        }
        public void AddRange(List<TEntity> entities, bool isCommit = true)
        {
            try
            {
                _context.AddRange(entities);
                if (isCommit)
                    _context.SaveChanges();


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void DeleteRange(List<TEntity> entities)
        {
            try
            {
                _context.RemoveRange(entities);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual Guid ParseGuid(string guidStr)
        {
            try
            {
                return Guid.Parse(guidStr);
            }
            catch { return Guid.Empty; }
        }

        public void SoftDeleteRange(List<TEntity> entities)
        {
            foreach (var item in entities)
            {
                item.IsDeleted = true;

            }
            _context.UpdateRange(entities);
            _context.SaveChanges();
        }

        public DbSet<TEntity> GetSet()
        {
            return _context.CreateSet<TEntity>();
        }

        public void ClearTracker()
        {
            _context.ChangeTracker.Clear();
        }

        public TEntity? Get(Guid id)
        {
            return GetSet().Find(id);
        }

        public Task<int> CountRecordsAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public void BulkInsert(IList<TEntity> items, int packageSize = 1000)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                _context.BulkInsert(items, new BulkConfig { BatchSize = packageSize });
                transaction.Commit();
            }
        }

        public async Task BulkInsert(IList<TEntity> entities, CancellationToken cancellationToken)
        {
            await _context.BulkInsertAsync(entities, cancellationToken: cancellationToken);
        }

        public async Task BulkUpdate(IList<TEntity> entities, CancellationToken cancellationToken)
        {
            await _context.BulkUpdateAsync(entities, cancellationToken: cancellationToken);
        }

        public IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate)
        {
            return GetSet().Where(predicate).AsQueryable();
        }

        public virtual IQueryable<TEntity> AsQueryable()
        {
            return GetSet().AsQueryable();
        }

		#endregion
		public IQueryable<TEntity> FindByPredicate(Expression<Func<TEntity, bool>> predicate)
		{
			return GetSet().Where(predicate).AsQueryable();
		}
		public int CountRecordsByPredicate(Expression<Func<TEntity, bool>> predicate)
		{
			return GetSet().Where(predicate).Count();
		}
        public IQueryable<TEntity> addSort(IQueryable<TEntity> input, SortByInfo sortByInfo)
        {
            var result = input.AsQueryable();
            var type = sortByInfo.FieldName;
            type = char.ToUpper(type[0]) + type.Substring(1);

            var param = Expression.Parameter(typeof(TEntity), "m");
            var property = Expression.Property(param, type);
            var lambda = Expression.Lambda<Func<TEntity, object>>(Expression.Convert(property, typeof(object)), param);


            if (sortByInfo.Ascending != null && sortByInfo.Ascending.Value)
            {
                result = result.OrderBy(lambda);
            }
            else
            {
                result = result.OrderByDescending(lambda);
            }

            return (IQueryable<TEntity>)result;
        }

        public void EditRange(List<TEntity> entities)
        {
            _context.UpdateRange(entities);
        }
    }
}
