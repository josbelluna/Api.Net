using System.Linq;
using Api.Models;

namespace Api.Services
{
    public interface IListService
    {
        ListResult GetList<TDto>(IService<TDto> list, ListParameters parameters);
        ListResult GetList<TDto>(IService<TDto> list);
    }
}