using Api.Exceptions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Models
{
    public class Validator<T> : AbstractValidator<T>
    {
        private readonly List<string> _errors;
        public T Instance { get; }
        public Validator(T dto)
        {
            _errors = new List<string>();
            Instance = dto;
        }

        public void AddError(string message)
        {
            _errors.Add(message);
        }
        public void RemoveError(string message)
        {
            _errors.Remove(message);
        }
        public void Validate()
        {
            var results = Validate(Instance);
            _errors.AddRange(results.Errors.Select(t => t.ErrorMessage));
            if (_errors.Any())
                throw new ValidateException(string.Join("\n", _errors));
        }
    }
    public class Validator
    {
        private readonly List<string> _errors;
        public Validator()
        {
            _errors = new List<string>();
        }

        public void AddError(string message)
        {
            _errors.Add(message);
        }
        public void RemoveError(string message)
        {
            _errors.Remove(message);
        }
        public void Validate()
        {
            if (_errors.Any())
                throw new ValidateException(string.Join("\n", _errors));
        }
    }
}
