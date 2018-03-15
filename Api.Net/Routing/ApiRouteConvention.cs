using Api.Controllers;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Routing
{
    public class ApiRouteConvention : IApplicationModelConvention
    {
        private readonly AttributeRouteModel _centralPrefix;

        public ApiRouteConvention(IRouteTemplateProvider routeTemplateProvider)
        {
            _centralPrefix = new AttributeRouteModel(routeTemplateProvider);
        }

        public void Apply(ApplicationModel application)
        {
            var apiControllers = application.Controllers.Where(t => t.ControllerType.IsGenericType && t.ControllerType.GetGenericTypeDefinition() == typeof(ApiController<>)).ToArray();
            foreach (var controller in apiControllers)
            {
                var unmatchedSelectors = controller.Selectors.Where(x => x.AttributeRouteModel == null).ToList();
                if (!unmatchedSelectors.Any()) return;
                foreach (var selectorModel in unmatchedSelectors)
                {
                    selectorModel.AttributeRouteModel = _centralPrefix;
                }
            }
        }
    }
}
