using Api.Dto.Autommaper;
using Api.Dto.Interfaces;
using System.Data.Entity;
using Api.Models;

namespace Api.Dto.Base
{
    public abstract class Dto<TDto, TEntity> : IDto<TDto, TEntity>, IDtoEvent<TDto>, IDtoValidator<TDto> where TDto : class where TEntity : class
    {
        public virtual void Map(IMap<TDto, TEntity> mapper) { }
        public virtual void MapBack(IMap<TEntity, TDto> mapper) { }

        public virtual void BeforeGet(TDto dto) { }
        public virtual void BeforeSave(TDto dto) { }
        public virtual void AfterSave(TDto dto) { }
        public virtual void BeforeUpdate(TDto dto) { }
        public virtual void AfterUpdate(TDto dto) { }
        public virtual void BeforeInsert(TDto dto) { }
        public virtual void AfterInsert(TDto dto) { }

        public virtual void ValidateInsert(TDto dto, Error error) { }
        public virtual void ValidateUpdate(TDto dto, Error error) { }
        public virtual void ValidateSave(TDto dto, Error error) { }
    }
}
