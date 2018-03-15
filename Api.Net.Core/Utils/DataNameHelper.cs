using System.Linq;
using System.Reflection;
using Api.Attributes;

namespace Api.Utils
{
    public static class DataNameHelper
    {
        public static string GetDataName(this PropertyInfo property, string defaultName = null)
        {
            var attribute = property.GetCustomAttributes().OfType<DataNameAttribute>().FirstOrDefault();
            return attribute != null ? attribute.Name : (defaultName ?? property.Name);
        }
    }
}
