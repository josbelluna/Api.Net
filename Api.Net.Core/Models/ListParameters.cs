using System.Collections.Generic;

namespace Api.Models
{
    public class ListParameters
    {
        public Dictionary<string, object> Filters { get; set; }
        public IEnumerable<string> Selection { get; set; }
        public IEnumerable<string> Exclusion { get; set; }
        public IEnumerable<string> Orders { get; set; }
        public bool Descending { get; set; }
        public int? CurrentPage { get; set; }
        public int? PageSize { get; set; }
    }
}
