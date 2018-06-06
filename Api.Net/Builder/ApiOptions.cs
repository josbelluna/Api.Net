using Api.Net.Core.Conventions;
using Microsoft.EntityFrameworkCore;
using System;

namespace Api.Builder
{
    public class ApiOptions
    {
        public ApiOptions()
        {
            Conventions = new ApiConvention();
        }
        public string RoutePrefix { get; set; } = "api";
        public Func<DbContextOptionsBuilder, string, DbContextOptionsBuilder> ContextOption { get; set; }
        public ApiConvention Conventions { get; }
    }
}