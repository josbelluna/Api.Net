using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Net.Utils
{
    public static class GenericUtils
    {
        public static int CountGenericArguments(this Type type)
        {
            return type.IsGenericType ? type.GetGenericArguments().Length : 0;
        }
    }
}
