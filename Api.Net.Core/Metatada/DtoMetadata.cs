using Api.Net.Core.Conventions;
using Api.Net.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Api.Net.Core.Metatada
{
    public class DtoMetadata
    {
        private static DtoMetadata _instance;
        public static DtoMetadata Instance
        {
            get
            {
                if (_instance is null)
                    _instance = new DtoMetadata();
                return _instance;
            }
        }
        private DtoMetadata()
        {
            Projections = new Dictionary<Type, IEnumerable<ProjectionDefinition>>();
        }

        public Dictionary<Type, IEnumerable<ProjectionDefinition>> Projections { get; set; }
        public ApiConvention Convention { get; set; }

        public ProjectionDefinition ResolveProyection(Type dtoType, string name)
        {
            if (!Projections.ContainsKey(dtoType)) return null;
            return Projections[dtoType].FirstOrDefault(t => t.Name == name);
        }
    }
}
