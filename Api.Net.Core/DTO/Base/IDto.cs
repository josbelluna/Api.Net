using Api.Dto.Autommaper;
using Api.Dto.Interfaces;

namespace Api.Dto.Base
{
    public interface IDto<TDto, TEntity> where TDto : class where TEntity : class
    {
        void MapExpandables(IMap<TDto, TEntity> config);
        void Map(IMap<TDto, TEntity> config);
        void MapBack(IMap<TEntity, TDto> config);

    }
}
