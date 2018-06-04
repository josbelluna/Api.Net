using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Net.Core.Models
{
    internal class DtoRelation
    {
        internal string RelationColumnName { get; set; }
        internal Type DtoType { get; set; }
    }
}
