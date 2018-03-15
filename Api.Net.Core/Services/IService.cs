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
        TDto Find(object key);

        TDto Add(TDto dto);
        void AddRange(IEnumerable<TDto> dtos);
        TDto Update(TDto dto);
        TDto PartialUpdate(object id, TDto dto);

        void ValidateDto(TDto dto, Error errors);
        void ValidateAdd(TDto dto, Error errors);
        void ValidateUpdate(TDto dto, Error errors);
        void ValidateDelete(int id, Error errors);


        void Activate(TDto dto);
        TDto Delete(int id, string applicationUser);
    }
    public interface IService<TDto, TEntity> : IService<TDto> { }
}
