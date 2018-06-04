using System;

namespace Api.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExpandableAttribute : Attribute
    {

    }
}
