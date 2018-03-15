using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Parameters
{
    public class ApiParameter
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public string OrderBy { get; set; }
        public string Fields { get; set; }
        public string Exclude { get; set; }
        public bool Descending { get; set; }
    }
}
