using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Exceptions
{
    public class ValidateException : Exception
    {
        public ValidateException() : base() { }
        public ValidateException(string message) : base(message) { }

        public ValidateException(string message,string codigo) : base(message) {
            this.Codigo = codigo;
        }


        public IEnumerable<string> Errors { get; set; }
        public string Codigo { get; set; }             
    }
}
