using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    internal class ApiControllerModelConventionAttribute : Attribute, IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {

            var dtoType = controller.ControllerType.GenericTypeArguments[0];
            var attr = dtoType.GetCustomAttributes(false).OfType<ApiEndpointAttribute>().FirstOrDefault();
            controller.ControllerName = attr?.Endpoint ?? dtoType.Name.Replace("Dto", "");
        }
    }
}
