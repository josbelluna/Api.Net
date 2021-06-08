using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Parameters
{
    public class ApiParameter
    {
        public ApiParameter()
        {
            this.Page = 1;
            this.PageSize = 10;
            this.Descending = true;
        }

        public ApiParameter(int page, int pageSize, bool descending)
        {
            this.Page = page < 1 ? 1 : page;
            this.PageSize = pageSize > 10 ? 10 : pageSize;
            this.Descending = descending;
        }

        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public string OrderBy { get; set; }
        public string Fields { get; set; }
        public string Exclude { get; set; }
        public bool Descending { get; set; }
        public string Expand { get; set; }
        public string Project { get; set; }
    }
}
