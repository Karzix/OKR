﻿using OKR.Common.Models;
using OKR.Common.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace OKR.Common.Repository
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

        void AddRange(List<T> entities, bool isCommit = true);

        void DeleteRange(List<T> entities);
        void SoftDeleteRange(List<T> entities);
        Task<int> CountRecordsAsync(Expression<Func<T, bool>> predicate);
        void BulkInsert(IList<T> items, int packageSize = 1000);
        Task BulkInsert(IList<T> entities, CancellationToken cancellationToken);
        Task BulkUpdate(IList<T> entities, CancellationToken cancellationToken);
        //IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
        IQueryable<T> GetAll();
        IQueryable<T> FindByPredicate(Expression<Func<T, bool>> predicate);
        int CountRecordsByPredicate(Expression<Func<T, bool>> predicate);
    }
}
