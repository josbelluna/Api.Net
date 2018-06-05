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
        TDto Update(TDto dto);
        TDto PartialUpdate(object id, TDto dto);

        void ValidateDto(Validator<TDto> validator);
        void ValidateAdd(Validator<TDto> validator);
        void ValidateUpdate(Validator<TDto> validator);
        void ValidateDelete(int id, Validator validator);


        void Activate(TDto dto);
        TDto Delete(int id, string applicationUser);
    }
    public interface IService<TDto, TEntity> : IService<TDto> { }
}
