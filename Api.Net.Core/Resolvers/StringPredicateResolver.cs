using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Api.Models;

namespace Api.Resolvers
{
    public class StringPredicateResolver
    {
        public Predicate GetPredicate(string value, string sign)
        {
            if (value.Contains("'"))
                return ResolveSimpleLike(value, sign);
            if (value.StartsWith("*"))
                return ResolveSpaceLike(value, sign);
            if (value.Contains("$"))
                return ResolveMultipleLike(value, sign);
            return ResolveDefault(value, sign);
        }

        private static Predicate ResolveSimpleLike(string value, string sign)
        {
            return new Predicate(value.Replace("'", ""), "{0}.Contains(\"{1}\")");
        }
        private static Predicate ResolveSpaceLike(string value, string sign)
        {
            var newValue = value.Replace("*", "");
            var format = string.Join(" && ", newValue.Split(' ').Select(t => $"{{0}}.Contains(\"{t}\")"));
            return new Predicate(newValue, format);
        }
        private static Predicate ResolveMultipleLike(string value, string sign)
        {
            string[] operations = { "StartsWith", "Contains", "EndsWith" };
            var newValue = value.Replace("$", "");
            var parts = value.Split('$').Where((t, i) => i <= 2);
            var format = string.Join(" && ", parts.Select((t, i) => $"{{0}}.{operations[i]}(\"{t}\")"));
            return new Predicate(newValue, format);
        }
        private static Predicate ResolveDefault(string value, string sign)
        {
            return new Predicate(value, $"{{0}}{sign}\"{{1}}\"");
        }
    }
}
