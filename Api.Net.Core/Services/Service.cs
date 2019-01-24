using System;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Api.Dto.Interfaces;
using Api.Repositories;
using Api.Utils;
using Api.Models;
using System.Collections.Generic;
using Api.Dto.Base;
using Api.Net.Core.Utils;
using Api.Exceptions;

namespace Api.Services
{
    public class Service<TDto, TEntity> : IService<TDto> where TEntity : class where TDto : class
    {
        public Service(IRepository<TEntity> repository)
        {
            this.Repository = repository;
        }

        public IRepository<TEntity> Repository { get; protected set; }

        public IQueryable<TDto> Dto
        {
            get
            {
                return GetDto();
            }
        }
        public virtual IQueryable<TDto> GetDto()
        {
            return this.Repository.Entities.ProjectTo<TDto>();
        }
        public virtual IQueryable<TDto> GetDto(params string[] membersToExpand)
        {
            return this.Repository.Entities.ProjectTo<TDto>(null, membersToExpand);
        }
        public virtual TDto Find(object key)
        {
            if (key is null) throw new ValidateException("Resource not found");
            var entity = this.Repository.Find(key);
            var dto = Mapper.Map<TDto>(entity);
            (dto as IDtoEvent<TDto>)?.BeforeGet(dto);
            return dto;
        }

        public TDto Delete(object id)
        {
            if (id is null) throw new ValidateException("Resource not found");
            var validator = new Validator();
            ValidateDelete(id, validator);
            validator.Validate();

            var entity = this.Repository.Delete(id);
            if (entity == null) throw new ValidateException("Resource not found");
            return Mapper.Map<TDto>(entity);
        }

        public virtual TDto Add(TDto dto)
        {
            if (dto is null) throw new ValidateException("Resource not found");
            var dtoEventHandler = dto as IDtoEvent<TDto>;
            if (dtoEventHandler != null)
            {
                dtoEventHandler.BeforeSave(dto);
                dtoEventHandler.BeforeInsert(dto);

            }
            var validator = new Validator<TDto>(dto);
            var dtoValidator = dto as IDtoValidator<TDto>;
            if (dtoValidator != null)
            {
                dtoValidator.ValidateSave(validator);
                dtoValidator.ValidateInsert(validator);
            }
            ValidateDto(validator);
            ValidateAdd(validator);
            validator.Validate();

            var entity = Mapper.Map<TEntity>(dto);
            this.Repository.Add(entity);

            dto = Mapper.Map<TDto>(entity);
            if (dtoEventHandler != null)
            {
                dtoEventHandler.AfterSave(dto);
                dtoEventHandler.AfterInsert(dto);
            }
            return dto;
        }

        public virtual void AddRange(IEnumerable<TDto> dtos)
        {
            if (dtos is null) throw new ValidateException("Resource not found");
            Repository.RestrictSave();
            foreach (var dto in dtos) Add(dto);
            Repository.EnableSave();
            Repository.SaveChanges();
        }

        public virtual TDto Update(object key, TDto dto)
        {
            if (key is null || dto is null) throw new ValidateException("Resource not found");
            var dtoEventHandler = dto as IDtoEvent<TDto>;
            if (dtoEventHandler != null)
            {
                dtoEventHandler.BeforeSave(dto);
                dtoEventHandler.BeforeUpdate(dto);
            }
            var entity = this.Repository.Find(key);
            Mapper.Map(dto, entity);
            dto = Mapper.Map<TDto>(entity);

            var validator = new Validator<TDto>(dto);
            var dtoValidator = dto as IDtoValidator<TDto>;
            if (dtoValidator != null)
            {
                dtoValidator.ValidateSave(validator);
                dtoValidator.ValidateUpdate(validator);
            }

            ValidateDto(validator);
            ValidateUpdate(validator);
            validator.Validate();

            this.Repository.Update(entity);
            dto = Mapper.Map<TDto>(entity);
            if (dtoEventHandler != null)
            {
                dtoEventHandler.AfterSave(dto);
                dtoEventHandler.AfterUpdate(dto);
            }
            return dto;
        }

        public virtual void ValidateDto(Validator<TDto> validator) { }
        public virtual void ValidateAdd(Validator<TDto> validator) { }
        public virtual void ValidateUpdate(Validator<TDto> validator) { }
        public virtual void ValidateDelete(object id, Validator validator) { }

        public void Dispose()
        {
            this.Repository?.Dispose();
            this.Repository = null;
            Clear();
        }
        public virtual void Clear() { }

    }
}
