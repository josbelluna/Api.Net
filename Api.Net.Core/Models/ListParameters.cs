using System.Collections.Generic;

namespace Api.Models
{
    public class ListParameters
    {
        public ListParameters()
        {
            this.CurrentPage = 1;
            this.PageSize = 10;
            this.Descending = true;
        }

        public ListParameters(int currentPage, int pageSize, bool descending)
        {
            this.CurrentPage = currentPage < 1 ? 1 : currentPage;
            this.PageSize = pageSize > 10 ? 10 : pageSize;
            this.Descending = descending;
        }

        public Dictionary<string, object> Filters { get; set; }
        public IEnumerable<string> Selections { get; set; }
        public IEnumerable<string> Exclusions { get; set; }
        public IEnumerable<string> Expansions { get; set; }
        public string Projection { get; set; }
        public IEnumerable<string> Orders { get; set; }
        public bool Descending { get; set; }
        public int? CurrentPage { get; set; }
        public int? PageSize { get; set; }
    }
}
