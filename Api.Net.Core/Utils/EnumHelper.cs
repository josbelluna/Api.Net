using System;
using System.Linq;
using System.Reflection;

namespace Api.Utils
{
    public class KeyAttribute : Attribute
    {
        public string Name { get; }
        public KeyAttribute(string name)
        {
            this.Name = name;
        }
    }
    public static class EnumHelper
    {
        public static string GetKey(this object o)
        {
            var fields = o.GetType().GetFields();

            var field = fields.FirstOrDefault(t => t.GetValue(o).Equals(o));
            if (field == null) return null;

            var attribute = field.GetCustomAttribute(typeof(KeyAttribute)) as KeyAttribute;
            return attribute != null ? attribute.Name : field.Name;
        }
        public static string GetMemberByKey<TEnum>(string key)
        {
            var fields = typeof(TEnum).GetFields();

            var attributes = fields.Select(t => new
            {
                Name = t.Name,
                Attribute = (KeyAttribute)t.GetCustomAttribute(typeof(KeyAttribute))
            });

            return attributes.FirstOrDefault(t => t.Attribute.Name == key)?.Name;
        }
    }

}
