using Api.Dto.Autommaper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using System.Linq.Expressions;

namespace Api.Dto.Autommaper
{
    public class Maps<TDestination, TSource> : IMap<TDestination, TSource> where TDestination : class where TSource : class
    {
        public IMappingExpression<TSource, TDestination> Expression { get; }

        public Maps(IMappingExpression<TSource, TDestination> expression)
        {
            this.Expression = expression;
        }
        public IMap<TDestination, TSource> Map<TDestinationProperty, TSourceProperty>(
            Expression<Func<TDestination, TDestinationProperty>> destinationProperty,
            Expression<Func<TSource, TSourceProperty>> sourceProperty)
        {
            Expression.ForMember(destinationProperty, opt => opt.MapFrom(sourceProperty));
            return this;
        }
    }
}
