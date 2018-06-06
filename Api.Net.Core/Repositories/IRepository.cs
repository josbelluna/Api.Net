using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Api.Repositories
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        IQueryable<TEntity> Entities { get; }
        TEntity Find(object key);
        void Update(TEntity entity);

        TEntity Delete(TEntity entity);
        TEntity Delete(object id);

        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        void UpdateRange(IEnumerable<TEntity> entities);

        void RestrictSave();
        void EnableSave();
        void SaveChanges();
    }
}