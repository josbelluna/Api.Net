using Api.Dto.Autommaper;
using Api.Dto.Interfaces;
using System.Data.Entity;

namespace Api.Dto.Base
{
    public interface IDto<TDto, TEntity> where TDto : class where TEntity : class
    {
        void Map(IMap<TDto, TEntity> config);
        void MapBack(IMap<TEntity, TDto> config);

    }
}
