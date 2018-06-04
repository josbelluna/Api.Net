using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Api.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Api.Repositories
{
    public class Repository<TContext, TEntity> : IRepository<TEntity> where TEntity : class where TContext : DbContext
    {
        public TContext Context { get; }
        public Repository(TContext context)
        {
            Context = context;
        }

        public IQueryable<TEntity> Entities => GetEntities();

        public virtual IQueryable<TEntity> GetEntities()
        {
            return this.Context.Set<TEntity>().AsNoTracking();
        }

        public virtual TEntity Find(object key)
        {
            var entity = this.Context.Set<TEntity>().Find(key);
            return entity;
        }
        public virtual void Add(TEntity entity)
        {
            this.AttachProperties(entity);
            this.Context.Set<TEntity>().Add(entity);
            SaveChanges();
        }

        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            this.Context.Set<TEntity>().AddRange(entities);
            SaveChanges();
        }
        public virtual void Update(TEntity entity)
        {
            this.Context.Entry(entity).State = EntityState.Modified;
            SaveChanges();
        }
        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities) Update(entity);
        }
        public virtual void Remove(TEntity entity)
        {
            this.Context.Set<TEntity>().Remove(entity);
            SaveChanges();
        }
        public virtual void Remove(object id)
        {
            var entity = this.Find(id);
            this.Context.Set<TEntity>().Remove(entity);
            SaveChanges();
        }
        public virtual void Delete(TEntity entity)
        {
            this.Attach(entity, EntityState.Deleted);

            this.SaveChanges();
        }

        public TEntity Delete(int id, string user = "SYSTEM")
        {
            var _obj = Find(id);
            var type = _obj.GetType();

            var active = type.GetProperty("Active");

            if (active != null)
            {
                active.SetValue(_obj, false);
                Update(_obj);
                return _obj;
            }
            Delete(_obj);
            return _obj;
        }

        public virtual void SaveChanges()
        {
            this.Context.SaveChanges();
        }

        public virtual EntityEntry GetEntry(TEntity entity)
        {
            var entry = this.Context.ChangeTracker.Entries<TEntity>().FirstOrDefault(t => t.Entity.Equals(entity));

            if (entry == null)
                entry = this.Context.Entry(entity);

            return entry;
        }
        public virtual void AttachProperties(object entity, EntityState entryState = EntityState.Unchanged)
        {
            Type _type = entity.GetType();
            foreach (var property in _type.GetProperties())
            {
                try
                {
                    var entry = this.Context.Entry(property.GetValue(entity));
                    entry.State = entryState;
                }
                catch
                {
                    //TODO
                }

            }
        }
        public virtual TEntity Attach(TEntity entity, EntityState estate)
        {
            this.GetEntry(entity).State = estate;
            return entity;
        }
        public virtual void DetachAll()
        {

            foreach (EntityEntry dbEntityEntry in this.Context.ChangeTracker.Entries())
            {

                if (dbEntityEntry.Entity != null)
                {
                    dbEntityEntry.State = EntityState.Detached;
                }
            }
        }

        private bool _isDisposed;

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}
