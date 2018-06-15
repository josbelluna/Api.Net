using Api.Dto.Base;
using Api.Repositories;
using Api.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Api.Routing;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Api.Net.Core.Services;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Api.Attributes;
using Api.Net.Core.Metatada;
using Api.Net.Core.Models;

namespace Api.Builder
{
    internal class ApiBuilder
    {
        internal IMvcBuilder MvcBuilder { get; }
        internal IServiceCollection Services { get; }
        internal ApiOptions Options { get; }
        internal ApiBuilder(IMvcBuilder mvcBuilder, ApiOptions config)
        {
            this.MvcBuilder = mvcBuilder;
            this.Services = mvcBuilder.Services;
            this.Options = config;
        }

        public ApiBuilder AddControllers()
        {
            Services.Configure<MvcOptions>(opt =>
            {
                opt.Conventions.Insert(0, new ApiRouteConvention(new RouteAttribute(Options.RoutePrefix + "/[controller]")));
            });
            MvcBuilder.ConfigureApplicationPartManager(p => p.FeatureProviders.Add(new ApiControllerFeatureProvider()));
            return this;
        }
        public ApiBuilder AddDtoMaps()
        {
            Mapper.Initialize(c =>
            {
                c.ResolveDtoMaps();
            });
            ResolveDtoProjections();
            return this;
        }

        private void ResolveDtoProjections()
        {
            var dtos = MapperUtils.GetAllDtos();
            foreach (var dto in dtos)
            {
                var projections = dto.GetInterfaces()
                 .Select(t =>
                 new ProjectionDefinition
                 {
                     ProjectionType = t,
                     Name = t.GetCustomAttribute<DtoProjectionAttribute>()?.ProjectionName,
                     ProjectionProperties = t.GetProperties().Select(p => p.Name)
                 })
                 .Where(t => t.Name != null);
                if (!projections.Any()) continue;
                DtoMetadata.Instance.Projections.Add(dto, projections);
            }
        }
        public ApiBuilder AddConventions()
        {
            DtoMetadata.Instance.Convention = Options.Conventions;
            return this;
        }
        public ApiBuilder AddDtoServices()
        {
            var dtos = MapperUtils.GetAllDtos();
            foreach (var type in dtos)
            {
                var iServiceType = typeof(IService<>).MakeGenericType(type);
                if (Services.Any(t => t.ServiceType == iServiceType)) continue;

                var interfaces = type.GetInterfaces().Where(t2 => t2.IsGenericType && t2.GetGenericTypeDefinition() == typeof(IDto<,>));
                foreach (var @interface in interfaces)
                {
                    var genericArguments = @interface.GetGenericArguments();
                    var genericService = typeof(Service<,>).MakeGenericType(genericArguments[0], genericArguments[1]);
                    Services.AddScoped(iServiceType, genericService);
                }
            }
            return this;
        }
        public ApiBuilder AddApiServices()
        {
            var apiServices = MapperUtils.GetApiServices();
            Services.AddTransient<IListService, ListService>();
            Services.AddSingleton<IRelationalDtoService, RelationalDtoService>();
            InjectInterfaces(Services, apiServices);
            return this;
        }
        public ApiBuilder AddApiRepositories()
        {
            var repositories = MapperUtils.GetApiRepositories();
            InjectInterfaces(Services, repositories);
            return this;
        }
        public ApiBuilder AddEntitiesRepositories()
        {
            var contextEntities = MapperUtils.GetAllContextEntities();
            foreach (var contextEntity in contextEntities)
            {
                var @interface = typeof(IRepository<>).MakeGenericType(contextEntity.EntityType);
                if (Services.Any(t => t.ServiceType == @interface)) continue;
                Services.AddScoped(@interface, typeof(Repository<,>).MakeGenericType(contextEntity.DbContextType, contextEntity.EntityType));
            }
            return this;
        }
        public ApiBuilder AddDbContexts()
        {
            if (Options.ContextOption == null) return this;
            var contexts = MapperUtils.GetAllContext();
            foreach (var context in contexts)
            {
                var builder = new DbContextOptionsBuilder();
                Services.AddTransient(context, p =>
                  {
                      return Activator.CreateInstance(context, Options.ContextOption(builder, p.GetService<IConfiguration>().GetConnectionString(context.Name)).Options);
                  });
            }

            return this;
        }
        public IServiceCollection AddGeneric<TDto, TEntity>(IServiceCollection services) where TDto : class where TEntity : class
        {
            services.AddScoped<IService<TDto>, Service<TDto, TEntity>>();
            return services;
        }
        public IServiceCollection AddService<TService, TDto>(IServiceCollection services) where TService : class, IService<TDto> where TDto : class
        {
            services.AddTransient<IService<TDto>, TService>();
            return services;
        }
        private void InjectInterfaces(IServiceCollection services, IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                var interfaces = type.GetInterfaces();
                if (!services.Any(t => t.ServiceType == type))
                {
                    services.AddScoped(type);
                }
                foreach (var @interface in interfaces)
                {
                    if (services.Any(t => t.ServiceType == @interface)) continue;
                    services.AddScoped(@interface, type);
                }
            }
        }
    }
}
