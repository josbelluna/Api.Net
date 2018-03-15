using System;

namespace Api.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ApiService : Attribute
    {
      
    }
}
