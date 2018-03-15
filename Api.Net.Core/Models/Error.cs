using Api.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Models
{
    public class Error
    {
        private readonly List<string> _errors;
        public Error()
        {
            _errors = new List<string>();
        }
        public void Add(string message)
        {
            _errors.Add(message);
        }

        public void Remove(string message)
        {
            _errors.Remove(message);
        }
        public void Validate()
        {
            if (_errors.Any())
                throw new ValidateException(string.Join(",", _errors));
        }
    }
}
