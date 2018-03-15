using Api.Models;
using Api.Parameters;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Utils
{
    public static class ParameterUtils
    {
        public static Dictionary<string, object> ToDictionary(this IQueryCollection collection)
        {
            var dictionary = new Dictionary<string, object>();
            foreach (var key in collection.Keys)
            {
                dictionary[key] = collection[key].FirstOrDefault();
            }
            return dictionary;
        }
        public static ListParameters ProcessParameters(this ApiParameter apiParameters, IQueryCollection collection)
        {
            var parameters = new ListParameters
            {
                Filters = collection.ToDictionary(),
                Orders = apiParameters.OrderBy?.Split(','),
                Descending = apiParameters.Descending,
                PageSize = apiParameters.PageSize,
                CurrentPage = apiParameters.Page,
                Selection = apiParameters.Fields?.Split(','),
                Exclusion = apiParameters.Exclude?.Split(',')
            };

            return parameters;
        }     
    }
}
