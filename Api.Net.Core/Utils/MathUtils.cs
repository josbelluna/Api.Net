using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Utils
{
   public static class MathUtils
    {
        public static decimal Round(decimal a, int digits)
        {
            return Math.Round(a, digits, MidpointRounding.AwayFromZero);
        }
    }
}
