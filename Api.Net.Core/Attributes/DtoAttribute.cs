using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class DtoAttribute : Attribute
    {
        public readonly Type EntityType;

        public DtoAttribute(Type entityType)
        {
            EntityType = entityType;
        }
    }
}
