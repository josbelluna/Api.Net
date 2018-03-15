using Api.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Utils
{
    public static class RepositoryUtils
    {
        public static IRepository<TEntity> ResolveRepository<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity);
            var dbType = typeof(DbSet<TEntity>);
            var @namespace = type.Namespace;
            var types = type.Assembly.GetTypes().Where(t => t.Namespace == @namespace);
            foreach (var contextType in types)
            {
                if (!typeof(DbContext).IsAssignableFrom(contextType)) continue;
                if (!contextType.GetProperties().Any(t => t.PropertyType == dbType)) continue;
                return (IRepository<TEntity>)Activator.CreateInstance(typeof(Repository<,>).MakeGenericType(contextType, type));
            }
            return null;
        }
    }
}
