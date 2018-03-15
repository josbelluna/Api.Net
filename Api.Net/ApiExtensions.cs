using Api.Builder;
using Api.Services;
using Api.Utils;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
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
            builder.AddDtoMaps().AddControllers().AddApiRepositories().AddEntitiesRepositories().AddApiServices().AddDtoServices();
            return mvcBuilder;
        }

    }
}
