using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Utils
{
    public static class JsonPatchUtils
    {
        public static JsonPatchDocument ToJsonPatchDocument(this object changes)
        {
            var patch = new JsonPatchDocument();
            var properties = (changes as IEnumerable<KeyValuePair<string, JToken>>);
            foreach (var property in properties)
                patch.Replace($"/{property.Key}", property.Value);
            return patch;
        }
    }
}
