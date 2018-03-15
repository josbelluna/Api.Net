using System.Linq;
using Api.Models;
using Api.Utils;
using Api.Dto.Interfaces;
using Api.Enums;

namespace Api.Services
{
    public class ListService : IListService
    {
        public ListResult GetList<TEntity>(IQueryable<TEntity> entities, ListParameters parameters)
        {

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
            var selection = parameters.Selection;
            var selectedData = entities.Select(selection);

            //Exclude
            var exclusion = parameters.Exclusion;
            var excludedData = selectedData.Exclude<TEntity>(exclusion);

            var result = new ListResult
            {
                Count = count,
                Data = excludedData
            };

            return result;
        }      
    }
}