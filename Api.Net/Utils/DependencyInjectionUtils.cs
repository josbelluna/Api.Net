using Api.Enums;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Net.Utils
{
    public static class DependencyInjectionUtils
    {
        public static void AddServiceWithLifeTime(this IServiceCollection services, Type serviceType, Type implementationType, LifeTime? lifeTime)
        {
            if (serviceType.IsGenericType) serviceType = serviceType.GetGenericTypeDefinition();
            if (implementationType.IsGenericType) implementationType = implementationType.GetGenericTypeDefinition();

            if (lifeTime == null) lifeTime = LifeTime.Scoped;
            var serviceLifeTime = (ServiceLifetime)(int)lifeTime;

            services.Add(new ServiceDescriptor(serviceType, implementationType, serviceLifeTime));
        }
    }
}
