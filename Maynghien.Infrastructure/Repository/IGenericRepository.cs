using MayNghien.Infrastructure.Models;
using MayNghien.Infrastructure.Models.Entity;
using MayNghien.Infrastructure.Request.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Maynghien.Infrastructure.Repository
{
    public interface IGenericRepository<T, C, TUser> where T : BaseEntity 
        where C : BaseContext<TUser> where TUser:IdentityUser
    {
        void ClearTracker();
        DbSet<T> GetSet();
        T? Get(Guid id);
        void Add(T entity);

        void Delete(T entity);

        void Edit(T entity);
        void EditRange(List<T> entities);

        void AddRange(List<T> entities, bool isCommit = true);

        void DeleteRange(List<T> entities);
        void SoftDeleteRange(List<T> entities);
        Task<int> CountRecordsAsync(Expression<Func<T, bool>> predicate);
        void BulkInsert(IList<T> items, int packageSize = 1000);
        Task BulkInsert(IList<T> entities, CancellationToken cancellationToken);
        Task BulkUpdate(IList<T> entities, CancellationToken cancellationToken);
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
        IQueryable<T> AsQueryable();
        IQueryable<T> FindByPredicate(Expression<Func<T, bool>> predicate);
        int CountRecordsByPredicate(Expression<Func<T, bool>> predicate);
        IQueryable<T> addSort(IQueryable<T> input, SortByInfo sortByInfo);
    }
}
