using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Services
{
    public interface IService<TDto> : IDisposable
    {
        IQueryable<TDto> Dto { get; }
        IQueryable<TDto> GetDto(params string[] membersToExpand);

        TDto Find(object key);

        TDto Add(TDto dto);
        void AddRange(IEnumerable<TDto> dtos);
        TDto Update(object key, TDto dto);

        void ValidateDto(Validator<TDto> validator);
        void ValidateAdd(Validator<TDto> validator);
        void ValidateUpdate(Validator<TDto> validator);
        void ValidateDelete(object id, Validator validator);

        TDto Delete(object id);
    }
    public interface IService<TDto, TEntity> : IService<TDto> { }
}
