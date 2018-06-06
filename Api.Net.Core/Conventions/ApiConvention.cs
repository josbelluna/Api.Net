using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Net.Core.Conventions
{
    public class ApiConvention
    {
        public string IdentifierProperty { get; set; } = "Id";
        public string VersionProperty { get; set; } = "Version";
        public string ActiveProperty { get; set; } = "Active";
    }
}
