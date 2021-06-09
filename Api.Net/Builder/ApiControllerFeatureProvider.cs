using Api.Controllers;
using Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Api
{
    internal class ApiControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            foreach (var dtoTupe in MapperUtils.GetAllDtos())
            {
                var typeName = dtoTupe.Name + "Controller";
                if (feature.Controllers.Any(t => t.Name == typeName)) return;


                var controllerType = typeof(ApiController<>).MakeGenericType(dtoTupe);
                feature.Controllers.Add(controllerType.GetTypeInfo());

            }
        }
    }

}
