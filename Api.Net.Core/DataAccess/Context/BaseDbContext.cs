using Api.Net.Core.DataAccess.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Api.DataAcess.Context
{

    public abstract class BaseDbContext : DbContext
    {
        public BaseDbContext(DbContextOptions options) : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var contextType = GetType();
            var assembliesTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes());

            var entities = assembliesTypes.Where(t =>
            {
                var baseType = t.BaseType;
                return
                  !t.IsAbstract &&
                  !t.IsInterface &&
                  t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition().Equals(typeof(IBaseEntity<,>))) &&
                  t.BaseType.GetGenericArguments().Any(x => x.Equals(contextType));

            });
            var applyConfigurationMethod = typeof(ModelBuilder).GetMethods()
                .First(t => t.Name.Equals(nameof(modelBuilder.ApplyConfiguration)) &&
                t.GetParameters()[0].ParameterType.GetGenericTypeDefinition().Equals(typeof(IEntityTypeConfiguration<>)));
            foreach (var entity in entities)
            {
                applyConfigurationMethod.MakeGenericMethod(entity)
                    .Invoke(modelBuilder, new object[] { Activator.CreateInstance(entity) });
            }

            base.OnModelCreating(modelBuilder);
        }
    }

}
