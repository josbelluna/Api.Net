using System.Linq;
using Api.Models;
using Api.Utils;
using Api.Dto.Interfaces;
using Api.Enums;
using System.Reflection;
using System;
using System.Collections.Generic;

namespace Api.Services
{
    public class ListService : IListService
    {
        public ListResult GetList<TDto>(IService<TDto> service, ListParameters parameters)
        {
            var entities = parameters.Expansions.Any() ? service.GetDto(ResolveExpansions<TDto>(parameters.Expansions)) : service.Dto;
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
            var selection = parameters.Selections;
            var exclusion = parameters.Exclusions;
            var expansion = parameters.Expansions;
            var data = entities.Select(selection, exclusion, expansion);

            var result = new ListResult
            {
                Count = count,
                Data = data
            };

            return result;
        }

        private string[] ResolveExpansions<TDto>(IEnumerable<string> expansions)
        {
            var type = typeof(TDto);
            return expansions.Select(t => ListUtils.GetPropertyName<TDto>(t, out _, out _))
                                                   .Where(t => t != null).ToArray();
        }
    }
}