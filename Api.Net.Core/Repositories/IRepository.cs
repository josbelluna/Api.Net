using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Api.Repositories
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        IQueryable<TEntity> Entities { get; }
        TEntity Find(object key, bool detach = false);
        void Update(TEntity entity, bool save = true);
        void Remove(TEntity entity);
        void Remove(object id);
        void Add(TEntity entity, bool save = true);
        void DetachAll();
        void AddRange(IEnumerable<TEntity> entities);
        void UpdateRange(IEnumerable<TEntity> entities);
        void AttachProperties(object entity, EntityState entryState = EntityState.Unchanged);
        TEntity Attach(TEntity entity, EntityState estate);
        void SaveChanges();
        void Delete(TEntity entity, bool save = true);
        TEntity Delete(int id, string user);
    }
}