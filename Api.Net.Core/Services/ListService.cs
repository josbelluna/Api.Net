using System.Linq;
using Api.Models;
using Api.Utils;
using Api.Dto.Interfaces;
using Api.Enums;
using System.Reflection;
using System;
using System.Collections.Generic;
using Api.Net.Core.Metatada;

namespace Api.Services
{
    public class ListService : IListService
    {
        public ListResult GetList<TDto>(IService<TDto> service, ListParameters parameters)
        {
            var entities = parameters.Expansions.Any() ? service.GetDto(ResolveExpansions<TDto>(parameters.Expansions)) : service.Dto.DefaultIfEmpty();

            //Filter
            var filters = parameters.Filters;
            entities = entities.Filter(filters);

            //Order
            var orders = parameters.Orders;
            entities = entities.Sort(orders ?? new string[] { "Id" }, parameters.Descending ? SortType.Descending : SortType.Ascending);
            var count = entities.Count();

            //Paginate
            entities = entities.Paginate(parameters.CurrentPage, parameters.PageSize);

            //Select        
            var fields = ResolveProjectionProperties(typeof(TDto), parameters.Projection);

            parameters.Selections = fields.Any() ? fields : parameters.Selections;

            var data = entities.Select(parameters.Selections, parameters.Exclusions, parameters.Expansions);

            var result = new ListResult
            {
                Count = count,
                Data = data.EmptyIfNull()
            };

            return result;
        }

        private IEnumerable<string> ResolveProjectionProperties(Type type, string projection)
        {
            var p = DtoMetadata.Instance.ResolveProyection(type, projection);
            return p != null ? p.ProjectionProperties : new string[] { };
        }

        private string[] ResolveExpansions<TDto>(IEnumerable<string> expansions)
        {
            var type = typeof(TDto);
            return expansions.Select(t => ListUtils.GetPropertyName<TDto>(t, out _, out _))
                                                   .Where(t => t != null).ToArray();
        }
    }
}