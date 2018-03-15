using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models
{
    public class ListResult
    {
        public int Count { get; set; }
        public object Data { get; set; }
    }
}
