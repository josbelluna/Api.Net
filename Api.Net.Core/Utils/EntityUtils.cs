using Api.Net.Core.Metatada;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Net.Core.Utils
{
    public static class EntityUtils
    {

        public static bool SetVersion(object entity, int version)
        {
            var property = entity.GetType().GetProperty(DtoMetadata.Instance.Convention.VersionProperty);
            if (property == null) return false;
            property.SetValue(entity, version);
            return true;
        }

        public static int? GetVersion(object entity)
        {
            var property = entity.GetType().GetProperty(DtoMetadata.Instance.Convention.VersionProperty);
            if (property == null) return null;
            return (int)property.GetValue(entity);
        }

        public static bool SetActive(object entity, bool active)
        {
            var property = entity.GetType().GetProperty(DtoMetadata.Instance.Convention.ActiveProperty);
            if (property == null) return false;
            property.SetValue(entity, active);
            return true;
        }

        public static bool? GetActive(object entity)
        {
            var property = entity.GetType().GetProperty(DtoMetadata.Instance.Convention.ActiveProperty);
            if (property == null) return null;
            return (bool)property.GetValue(entity);
        }

        public static bool SetIdentifier(object entity, object id)
        {
            var property = entity.GetType().GetProperty(DtoMetadata.Instance.Convention.VersionProperty);
            if (property == null) return false;
            property.SetValue(entity, id);
            return true;
        }

        public static object GetIdentifier(object entity)
        {
            var property = entity.GetType().GetProperty(DtoMetadata.Instance.Convention.IdentifierProperty);
            if (property == null) return null;
            return property.GetValue(entity);
        }
        public static object ConvertIdentifier<T>(object id)
        {
            var property = typeof(T).GetProperty(DtoMetadata.Instance.Convention.IdentifierProperty);
            if (property == null) return id;
            return Convert.ChangeType(id, property.PropertyType);
        }
    }
}
