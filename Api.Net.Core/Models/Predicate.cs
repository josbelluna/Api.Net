using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Models
{
    public class Predicate
    {
        public Predicate(string value, string format)
        {
            this.Value = value;
            Format = format;
        }

        public string Value { get; set; }
        public string Format { get; set; }
    }
}
