using Api.Dto.Autommaper;
using Api.Dto.Interfaces;
using Api.Models;
using System.Linq;
using Api.Attributes;

namespace Api.Dto.Base
{
    public abstract class Dto<TDto, TEntity> : IDto<TDto, TEntity>, IDtoEvent<TDto>, IDtoValidator<TDto> where TDto : class where TEntity : class
    {
        public void MapExpandables(IMap<TDto, TEntity> mapper)
        {
            var expands = this.GetType().GetProperties().Where(p => p.IsDefined(typeof(ExpandableAttribute), true)).Select(t => t.Name);
            foreach (var expand in expands)
                mapper.Expression.ForMember(expand, opt => opt.ExplicitExpansion());
        }
        public virtual void Map(IMap<TDto, TEntity> mapper) { }
        public virtual void MapBack(IMap<TEntity, TDto> mapper) { }

        public virtual void BeforeGet(TDto dto) { }
        public virtual void BeforeSave(TDto dto) { }
        public virtual void AfterSave(TDto dto) { }
        public virtual void BeforeUpdate(TDto dto) { }
        public virtual void AfterUpdate(TDto dto) { }
        public virtual void BeforeInsert(TDto dto) { }
        public virtual void AfterInsert(TDto dto) { }

        public virtual void ValidateInsert(Validator<TDto> validator) { }
        public virtual void ValidateUpdate(Validator<TDto> validator) { }
        public virtual void ValidateSave(Validator<TDto> validator) { }
    }
}
