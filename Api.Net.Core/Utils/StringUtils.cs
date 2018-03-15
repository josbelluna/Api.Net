using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Api.Utils;

namespace Api.Utils
{
    public static class StringUtils
    {
        public static string TrimSpace(this string s)
        {
            return s.Replace(" ", "");
        }
        public static string Left(this string s, int lenght)
        {
            return s.Length > lenght ? s.Substring(lenght) : s;
        }

        public static string Truncate(this string s, int lenght)
        {
            return s.Length > lenght ? s.Substring(0, lenght) : s;
        }
        public static bool Match(this PropertyInfo property, string s2)
        {
            if (property == null || s2 == null) return false;
            var propertyName = property.GetDataName();
            bool match = propertyName.ToLower().Equals(s2.ToLower());
            return match;
        }
    }
}
