using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Api.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Api.Net.Core.Utils;
using Api.Exceptions;

namespace Api.Repositories
{
    public class Repository<TContext, TEntity> : IRepository<TEntity> where TEntity : class where TContext : DbContext
    {
        private bool _save = true;

        public TContext Context { get; }
        public Repository(TContext context)
        {
            Context = context;
        }

        public IQueryable<TEntity> Entities => GetEntities();

        public virtual IQueryable<TEntity> GetEntities()
        {
            return this.Context.Set<TEntity>();
        }

        public virtual TEntity Find(object key)
        {
            key = EntityUtils.ConvertIdentifier<TEntity>(key);
            var entity = this.Context.Set<TEntity>().Find(key);
            return entity;
        }
        public virtual void Add(TEntity entity)
        {
            EntityUtils.SetActive(entity, true);
            this.Context.Set<TEntity>().Add(entity);
            SaveChanges();
        }

        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities) EntityUtils.SetActive(entity, true);
            this.Context.Set<TEntity>().AddRange(entities);
            SaveChanges();
        }

        public virtual void Update(TEntity entity)
        {
            var version = EntityUtils.GetVersion(entity);
            if (version.HasValue) EntityUtils.SetVersion(entity, version.Value + 1);
            this.Context.Entry(entity).State = EntityState.Modified;
            SaveChanges();
        }
        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            RestrictSave();
            foreach (var entity in entities) Update(entity);
            EnableSave();
            SaveChanges();
        }
        public virtual TEntity Delete(TEntity entity)
        {
            var success = EntityUtils.SetActive(entity, false);
            if (!success) this.Context.Set<TEntity>().Remove(entity);

            SaveChanges();
            return entity;
        }
        public virtual TEntity Delete(object id)
        {
            var entity = this.Find(id);
            if (entity == null) return null;
            entity = Delete(entity);
            return entity;
        }

        public void RestrictSave() { _save = false; }
        public void EnableSave() { _save = true; }

        public virtual void SaveChanges()
        {
            if (_save) this.Context.SaveChanges();
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}
