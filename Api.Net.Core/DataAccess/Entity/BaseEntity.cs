using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Net.Core.DataAccess.Entity
{
    public abstract class BaseEntity<TEntity, TContext> : IBaseEntity<TEntity, TContext> where TContext : DbContext where TEntity : class
    {
        public abstract void Configure(EntityTypeBuilder<TEntity> builder);
    }
}
