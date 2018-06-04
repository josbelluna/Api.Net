using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Api.Utils
    {
    public static class SignUtils
        {
        private static readonly string[] _signs = { ">=", "<=", ">", "<", "!=", "!", "=" };
        private static readonly string[] _joinWithOr = { "=", "|", };
        public static string ExtractSign(ref string key, ref string value)
            {
            var _key = key;
            var _value = value;
            var selectedSign = "=";

            var match = _signs.FirstOrDefault(t => _key.EndsWith(t) || _value.StartsWith(t));
            if (match == null) return selectedSign.ResolveSign();

            key = key.Replace(match, "");
            value = value.Replace(match, "");
            selectedSign = match.ResolveSign();

            return selectedSign;
            }

        private static string ResolveSign(this string sign)
            {
            switch (sign)
                {
                case "!":
                    return "!=";
                case "=":
                    return "==";
                default:
                    return sign;
                }
            }

        public static string ResolveJoinSign(string sign)
            {
            return _joinWithOr.Contains(sign) ? " || " : " && ";
            }
        }
    }
