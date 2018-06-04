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

namespace Api.Services
{
    public class Service<TDto, TEntity> : IService<TDto> where TEntity : class where TDto : class
    {
        private IRepository<TEntity> repository;

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
            var entity = this.Repository.Find(key);
            var dto = Mapper.Map<TDto>(entity);
            (dto as IDtoEvent<TDto>)?.BeforeGet(dto);
            return dto;
        }

        public void IncrementVersion(TEntity entity)
        {
            var version = entity.GetType().GetProperty("Version");
            if (version == null || version.GetValue(entity) == null) return;
            version.SetValue(entity, (int)version.GetValue(entity) + 1);
        }
        public void ActivateEntity(TEntity entity)
        {
            var version = entity.GetType().GetProperty("Estado");
            version?.SetValue(entity, true);
        }
        public void Activate(TDto dto)
        {
            if (dto is IEstado)
                ((IEstado)dto).Estado = true;
        }

        public TDto Delete(int id, string applicationUser = "SYSTEM")
        {
            var errors = new Error();
            ValidateDelete(id, errors);
            errors.Validate();

            var entity = this.Repository.Delete(id, applicationUser);
            return Mapper.Map<TDto>(entity);
        }

        public virtual TDto Add(TDto dto)
        {
            var dtoEventHandler = dto as IDtoEvent<TDto>;
            if (dtoEventHandler != null)
            {
                dtoEventHandler.BeforeSave(dto);
                dtoEventHandler.BeforeInsert(dto);

            }
            var errors = new Error();
            var dtoValidator = dto as IDtoValidator<TDto>;
            if (dtoValidator != null)
            {
                dtoValidator.ValidateSave(dto, errors);
                dtoValidator.ValidateInsert(dto, errors);
            }
            ValidateDto(dto, errors);
            ValidateAdd(dto, errors);
            errors.Validate();

            Activate(dto);

            var entity = Mapper.Map<TEntity>(dto);
            IncrementVersion(entity);
            ActivateEntity(entity);
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
            foreach (var dto in dtos) Add(dto);
            Repository.SaveChanges();
        }
        public virtual TDto Update(TDto dto)
        {
            var dtoEventHandler = dto as IDtoEvent<TDto>;
            if (dtoEventHandler != null)
            {
                dtoEventHandler.BeforeSave(dto);
                dtoEventHandler.BeforeUpdate(dto);

            }
            var errors = new Error();
            var dtoValidator = dto as IDtoValidator<TDto>;
            if (dtoValidator != null)
            {
                dtoValidator.ValidateSave(dto, errors);
                dtoValidator.ValidateUpdate(dto, errors);
            }
            ValidateDto(dto, errors);
            ValidateUpdate(dto, errors);
            errors.Validate();

            var entity = Mapper.Map<TEntity>(dto);
            IncrementVersion(entity);
            this.Repository.Update(entity);

            dto = Mapper.Map<TDto>(entity);
            if (dtoEventHandler != null)
            {
                dtoEventHandler.AfterSave(dto);
                dtoEventHandler.AfterUpdate(dto);
            }
            return dto;
        }
        public virtual TDto PartialUpdate(object id, TDto dto)
        {
            var dtoEventHandler = dto as IDtoEvent<TDto>;
            if (dtoEventHandler != null)
            {
                dtoEventHandler.BeforeSave(dto);
                dtoEventHandler.BeforeUpdate(dto);
            }
            var entity = this.Repository.Find(id);
            Mapper.Map(dto, entity);
            dto = Mapper.Map<TDto>(entity);

            var errors = new Error();
            var dtoValidator = dto as IDtoValidator<TDto>;
            if (dtoValidator != null)
            {
                dtoValidator.ValidateSave(dto, errors);
                dtoValidator.ValidateUpdate(dto, errors);
            }

            ValidateDto(dto, errors);
            ValidateUpdate(dto, errors);
            errors.Validate();

            IncrementVersion(entity);
            this.Repository.Update(entity);
            dto = Mapper.Map<TDto>(entity);
            if (dtoEventHandler != null)
            {
                dtoEventHandler.AfterSave(dto);
                dtoEventHandler.AfterUpdate(dto);
            }
            return dto;
        }
        public virtual void ValidateDto(TDto dto, Error errors) { }
        public virtual void ValidateAdd(TDto dto, Error errors) { }
        public virtual void ValidateUpdate(TDto dto, Error errors) { }
        public virtual void ValidateDelete(int id, Error errors) { }

        public void Dispose()
        {
            this.Repository?.Dispose();
            this.Repository = null;
            Clear();
        }
        public virtual void Clear() { }

    }
}
