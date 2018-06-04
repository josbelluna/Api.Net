using Microsoft.EntityFrameworkCore;
using System;

namespace Api.Builder
{
    public class ApiOptions
    {
        public string RoutePrefix { get; set; } = "api";
        public Func<DbContextOptionsBuilder, string, DbContextOptionsBuilder> ContextOption { get; set; }
    }
}