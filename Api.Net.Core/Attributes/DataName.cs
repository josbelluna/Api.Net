using System;

namespace Api.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DataNameAttribute : Attribute
    {
        public DataNameAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; private set; }
    }
}
