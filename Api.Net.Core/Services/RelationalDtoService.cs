using Api.Attributes;
using Api.Net.Core.Models;
using Api.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Api.Net.Core.Services
{
    public class RelationalDtoService : IRelationalDtoService
    {
        internal Dictionary<string, DtoRelation> _relations { get; }

        public RelationalDtoService()
        {
            _relations = new Dictionary<string, DtoRelation>();
            ResolveRelations();
        }
        private void ResolveRelations()
        {
            var dtos = MapperUtils.GetAllDtos();
            foreach (var dto in dtos)
            {
                var endpoint = dto.GetCustomAttributes().OfType<ApiEndpointAttribute>().FirstOrDefault();
                if (endpoint is null) continue;
                var name = dto.BaseType.GetGenericArguments()[1].Name; //EntityTypeName
                _relations.Add(endpoint.Endpoint, new DtoRelation
                {
                    RelationColumnName = $"{name}Id",
                    DtoType = dto
                });
            }
        }
        public Tuple<string, string> TranslateRoute(string url)
        {
            var names = _relations.Keys;
            var model = ResolveControllers(url);
            if (model is null) return null;
            url = model.Endpoint;
            var parameters = GetParameters(model.Controllers);
            var queryString = string.Join("&", parameters);
            return Tuple.Create($"/api/{url}", $"?{queryString}");
        }

        private IEnumerable<string> GetParameters(IEnumerable<Tuple<string, string>> controllers)
        {
            foreach (var controller in controllers)
            {
                var columnName = _relations[controller.Item1].RelationColumnName;
                var id = controller.Item2;
                yield return $"{columnName}={id}";
            }
        }

        private ApiTranslationModel ResolveControllers(string path)
        {
            var pattern = $@"(/(?<cont>\w+)/(?<id>\d+))+/(?<end>\w+)";
            var p = new Regex(pattern);
            var reg = p.Match(path);
            if (!reg.Success) return null;
            var names = _relations.Keys;
            var controllers = reg.Groups["cont"].Captures.Cast<Capture>().Select(t => t.Value).Where(t => names.Contains(t));

            var ids = reg.Groups["id"].Captures.Cast<Capture>().Select(t => t.Value);

            var endpoint = reg.Groups["end"].Captures.Cast<Capture>().First().Value;
            var conts = controllers.Zip(ids, Tuple.Create);
            return new ApiTranslationModel { Endpoint = endpoint, Controllers = conts };
        }
    }
}


