using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Attributes
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class DtoProjectionAttribute : Attribute
    {
        public string ProjectionName { get; }

        public DtoProjectionAttribute(string proyectionName)
        {
            ProjectionName = proyectionName;
        }
    }
}
