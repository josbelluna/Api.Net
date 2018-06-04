using System.Collections.Generic;

namespace Api.Models
{
    public class ListParameters
    {
        public Dictionary<string, object> Filters { get; set; }
        public IEnumerable<string> Selections { get; set; }
        public IEnumerable<string> Exclusions { get; set; }
        public IEnumerable<string> Expansions { get; set; }
        public IEnumerable<string> Orders { get; set; }
        public bool Descending { get; set; }
        public int? CurrentPage { get; set; }
        public int? PageSize { get; set; }
    }
}
