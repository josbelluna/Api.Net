using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Api.Repositories;

namespace Api.Repositories
{
    public class Repository<TContext, TEntity> : IRepository<TEntity> where TEntity : class where TContext : DbContext, new()
    {
        private TContext _db;
        public TContext Context
        {
            get
            {
                _db = _db ?? new TContext();
                _db.Configuration.AutoDetectChangesEnabled = false;
                return _db;
            }
        }

        public IQueryable<TEntity> Entities => GetEntities();

        public virtual IQueryable<TEntity> GetEntities()
        {
            return this.Context.Set<TEntity>().AsNoTracking();
        }

        public virtual TEntity Find(object key, bool detach = false)
        {
            var entity = this.Context.Set<TEntity>().Find(key);
            if (detach) _db.Entry(entity).State = EntityState.Detached;
            return entity;
        }
        public virtual void Add(TEntity entity, bool save = true)
        {
            this.AttachProperties(entity);
            this.Context.Set<TEntity>().Add(entity);

            if (save) SaveChanges();
        }

        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            this.Context.Set<TEntity>().AddRange(entities);
            SaveChanges();
        }
        public virtual void Update(TEntity entity, bool save = true)
        {
            this.GetEntry(entity).State = EntityState.Modified;
            if (save) SaveChanges();
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
        public virtual void Delete(TEntity entity, bool save = true)
        {
            this.Attach(entity, EntityState.Deleted);

            if (save) this.SaveChanges();
        }

        public TEntity Delete(int id, string user = "SYSTEM")
        {
            var _obj = Find(id);
            var type = _obj.GetType();

            type.GetProperty("Estado")?.SetValue(_obj, false);
            type.GetProperty("FechaModificacion")?.SetValue(_obj, DateTime.Now);
            type.GetProperty("ModificadoPor")?.SetValue(_obj, user);

            var version = type.GetProperty("Version");
            version?.SetValue(_obj, ((int)version.GetValue(_obj)) + 1);
            Update(_obj);
            return _obj;
        }

        public virtual void SaveChanges()
        {
            this.Context.SaveChanges();
            DetachAll();
        }

        public virtual DbEntityEntry GetEntry(TEntity entity)
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

            foreach (DbEntityEntry dbEntityEntry in this.Context.ChangeTracker.Entries())
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
            Dispose(true);
        }
        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _db?.Dispose();
                    _db = null;
                }
                _isDisposed = false;
            }
        }

    }    
}
