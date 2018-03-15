using System.Linq;
using Api.Models;

namespace Api.Services
{
    public interface IListService
    {
        ListResult GetList<TEntity>(IQueryable<TEntity> list, ListParameters parameters);   
    }
}