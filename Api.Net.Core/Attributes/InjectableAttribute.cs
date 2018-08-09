using Api.Enums;
using System;

namespace Api.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public abstract class InjectableAttribute : Attribute
    {
        public LifeTime LifeTime { get; set; } = LifeTime.Scoped;
    }
}
