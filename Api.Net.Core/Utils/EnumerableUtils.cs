using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Utils
{
    public static class EnumerableUtils
    {
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> predicate)
        {
            if (list == null) return;
            foreach (var element in list) predicate(element);
        }
    }
}
