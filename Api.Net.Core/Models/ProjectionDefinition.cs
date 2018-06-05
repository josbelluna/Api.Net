using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Net.Core.Models
{
    public class ProjectionDefinition
    {
        public string Name { get; set; }
        public Type ProjectionType { get; set; }
        public IEnumerable<string> ProjectionProperties { get; set; }
    }
}
