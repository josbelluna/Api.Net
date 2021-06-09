using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ApiEndpointAttribute : Attribute
    {
        public ApiEndpointAttribute(string endpoint)
        {
            this.Endpoint = endpoint;
        }
        public string Endpoint { get; }
    }
}
