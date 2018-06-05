using Api.Builder;
using Api.Net.Core.Services;
using Api.Services;
using Api.Utils;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api
{
    public static class ApiExtensions
    {

        public static IMvcBuilder AddApi(this IMvcBuilder mvcBuilder, Action<ApiOptions> config = null)
        {
            var options = new ApiOptions();
            config?.Invoke(options);
            var builder = new ApiBuilder(mvcBuilder, options);
            builder.AddDbContexts().AddApiRepositories().AddEntitiesRepositories().AddDtoMaps().AddApiServices().AddDtoServices().AddControllers();
            mvcBuilder.AddJsonOptions(opt =>
            {
                opt.SerializerSettings.ContractResolver =
                    new CamelCasePropertyNamesContractResolver();
            });
            return mvcBuilder;
        }
        public static IApplicationBuilder UseApi(this IApplicationBuilder mvcBuilder)
        {
            mvcBuilder.Use((ctx, next) =>
            {
                var service = ctx.RequestServices.GetService<IRelationalDtoService>();
                var path = service.TranslateRoute(ctx.Request.Path);
                if (path != null)
                {
                    ctx.Request.Path = path.Item1;
                    ctx.Request.QueryString = new QueryString(path.Item2);
                }
                return next();
            });
            return mvcBuilder;
        }
    }
}
