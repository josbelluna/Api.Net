using Api;
using Api.Enums;
using Api.Resolvers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq;
using System.Linq.Dynamic;
using System.Reflection;
using System.Threading.Tasks;

namespace Api.Utils
{
    public static class ListUtils
    {
        public static IQueryable<TEntity> Filter<TEntity>(this IQueryable<TEntity> list, Dictionary<string, object> filters)
        {
            var keys = filters.Keys.ToList();

            foreach (var key in keys)
            {
                var value = filters[key];
                string predicate = GetPredicate<TEntity>(key, value.ToString());
                if (predicate == null) continue;

                var expression = DynamicExpression.ParseLambda<TEntity, bool>(predicate);
                list = list.Where(expression);
            }
            return list;
        }

        public static IQueryable<TEntity> Sort<TEntity>(this IQueryable<TEntity> list, IEnumerable<string> orders, SortType type = SortType.Ascending)
        {
            var dtoProperties = typeof(TEntity).GetTypeInfo().DeclaredProperties.Select(t => t.Name);

            var properties = from dtoProperty in dtoProperties
                             join order in orders
                             on dtoProperty.ToLower() equals order.ToLower()
                             select dtoProperty;

            if (!properties.Any()) return list;
            var orderType = (type == SortType.Ascending ? "Ascending" : "Descending");
            properties = properties.Select(t => $"{t} {orderType}");
            string query = string.Join(",", properties);

            return list.OrderBy(query);
        }
        public static IQueryable<TEntity> Paginate<TEntity>(this IQueryable<TEntity> list, int? page, int? pageSize)
        {
            if (pageSize == null) return list;

            if (page != null && page > 0) list = list.Skip((page.Value - 1) * pageSize.Value);

            return list.Take(pageSize.Value);
        }
        public static IEnumerable Select<TEntity>(this IEnumerable<TEntity> list, IEnumerable<string> fields = null)
        {
            var dtoProperties = typeof(TEntity).GetTypeInfo().DeclaredProperties.Select(t => t.Name);
            if (fields == null) return list;
            var propertyNames = new List<string>();
            foreach (var field in fields)
            {
                Type type;
                var propertyName = GetPropertyName<TEntity>(field, out type);
                if (propertyName == null) continue;
                propertyNames.Add(propertyName);
            }
            if (!propertyNames.Any()) return list;

            propertyNames = propertyNames.Select(t => t + " AS " + t.Replace(".", "")).ToList();
            string query = string.Join(",", propertyNames);
            query = string.Format("new({0})", query);
            return list.Select(query);
        }
        public static IEnumerable Exclude<TEntity>(this IEnumerable list, IEnumerable<string> fields = null)
        {
            if (fields == null) return list;
            var dtoProperties = typeof(TEntity).GetProperties().Select(t => t.Name);

            fields = fields.Select(t => t.ToLower());
            var diferentProperties = dtoProperties.Where(t => !fields.Contains(t.ToLower()));

            string query = string.Join(",", diferentProperties);
            query = string.Format("new({0})", query);
            return list.Select(query);
        }
        public static string GetPredicate<TEntity>(string filter, string value)
        {
            Type type;
            string sign = SignUtils.ExtractSign(ref filter, ref value);

            var propertyName = GetPropertyName<TEntity>(filter, out type);
            if (propertyName == null) return null;
            if (type.IsGenericType) type = type.GetGenericArguments()[0];
            return GetPredicate(type, propertyName, value, sign);
        }
        private static string GetPropertyName<TEntity>(string name, out Type type)
        {
            type = typeof(TEntity);
            var keys = name.ToLower().Split('.');
            var names = new List<string>();

            foreach (var key in keys)
            {
                var property = type.GetProperties().FirstOrDefault(t => t.Name.ToLower().Equals(key));
                if (property == null) return null;

                names.Add(property.Name);
                type = property.PropertyType;
            }

            return string.Join(".", names);
        }

        private static string GetPredicate(Type propertyType, string propertyName, object values, string sign)
        {
            IEnumerable<string> valueArray = values.ToString().Split(',');
            var predicates = new List<string>();
            foreach (var value in valueArray)
            {
                var _value = value;
                var format = "";

                if (propertyType == typeof(string))
                {
                    var resolver = new StringPredicateResolver();
                    var predicate = resolver.GetPredicate(_value, sign);
                    _value = predicate.Value;
                    format = predicate.Format;
                }
                else if (propertyType == typeof(DateTime))
                {
                    format = _value.GetDateFormat(sign);
                }
                else
                {
                    string[] split = value.Split('|');
                    if (split.Length > 1)
                    {
                        sign = "|";
                        format = $"{split[0]} <= {{0}} && {{0}} <= {split[1]}";
                    }
                    else
                    {
                        format = $"{{0}}{sign}{{1}}";
                    }
                }
                predicates.Add(string.Format(format, propertyName, _value));
            }
            return string.Join(SignUtils.ResolveJoinSign(sign), predicates);
        }
    }
}
