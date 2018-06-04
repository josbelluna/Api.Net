using Api;
using Api.Attributes;
using Api.Enums;
using Api.Resolvers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Dynamic.Core;
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
                list = list.Where(predicate);
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
        public static IQueryable Select<TEntity>(this IQueryable<TEntity> list, IEnumerable<string> selections, IEnumerable<string> exclusions, IEnumerable<string> expansions)
        {
            var dtoProperties = typeof(TEntity).GetTypeInfo().DeclaredProperties.Select(t => t.Name);
            var propertySet = new HashSet<string>();

            AddSelections<TEntity>(propertySet, selections);
            AddExclusions<TEntity>(propertySet, exclusions);
            AddExpansions<TEntity>(propertySet, expansions);
            var propertyNames = propertySet.Select(t => t + " AS " + t.Replace(".", ""));
            string query = string.Join(",", propertyNames);
            query = string.Format("new({0})", query);
            return list.Select(query);
        }

        private static void AddSelections<TEntity>(HashSet<string> propertyNames, IEnumerable<string> selections)
        {
            var props = typeof(TEntity).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                           .Where(p => !p.IsDefined(typeof(ExpandableAttribute), true)).Select(t => t.Name);

            if (!selections.Any()) { foreach (var prop in props) propertyNames.Add(prop); return; }

            Parallel.ForEach(selections, (name) =>
            {
                var prop = props.FirstOrDefault(t => t.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (prop != null) propertyNames.Add(prop);
            });
        }
        private static void AddExclusions<TEntity>(HashSet<string> propertyNames, IEnumerable<string> exclusions)
        {
            if (!exclusions.Any()) return;

            Parallel.ForEach(exclusions, (name) =>
            {
                var prop = GetPropertyName<TEntity>(name, out _, out _);
                if (prop != null) propertyNames.Remove(prop);
            });
        }
        private static void AddExpansions<TEntity>(HashSet<string> propertyNames, IEnumerable<string> expansions)
        {
            Parallel.ForEach(expansions, (name) =>
            {
                var prop = GetPropertyName<TEntity>(name, out _, out _);
                if (prop != null) propertyNames.Add(prop);
            });
        }

        public static string GetPredicate<TEntity>(string filter, string value)
        {
            Type type;
            string sign = SignUtils.ExtractSign(ref filter, ref value);

            var propertyName = GetPropertyName<TEntity>(filter, out type, out var isGeneric);
            if (propertyName == null) return null;
            if (type.IsGenericType) type = type.GetGenericArguments()[0];
            return GetPredicate(type, propertyName, value, sign, isGeneric);
        }
        public static string GetPropertyName<TEntity>(string name, out Type type, out bool isGeneric)
        {
            type = typeof(TEntity);
            isGeneric = false;
            var keys = name.ToLower().Split('.');
            var names = new List<string>();

            foreach (var key in keys)
            {
                var property = type.GetProperty(key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (property == null) return null;

                names.Add(property.Name);
                type = property.PropertyType;
                if (type.IsGenericType)
                {
                    type = type.GetGenericArguments()[0];
                    isGeneric = true;
                }
            }

            return string.Join(".", names);
        }

        private static string GetPredicate(Type propertyType, string propertyName, object values, string sign, bool isGeneric)
        {
            IEnumerable<string> valueArray = values.ToString().Split(',');
            var listParameter = "";
            if (isGeneric)
            {
                var part = propertyName.Split('.');
                listParameter = part.First();
                propertyName = propertyName.Replace($"{listParameter}.", "");
            }
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
                if (isGeneric)
                    format = $"{listParameter}.Any({format})";

                predicates.Add(string.Format(format, propertyName, _value));
            }
            return string.Join(SignUtils.ResolveJoinSign(sign), predicates);
        }
    }
}
