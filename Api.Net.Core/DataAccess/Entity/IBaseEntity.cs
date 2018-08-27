using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Net.Core.DataAccess.Entity
{
    public interface IBaseEntity<TEntity, TContext> : IEntityTypeConfiguration<TEntity> where TEntity : class
    {

    }
}
