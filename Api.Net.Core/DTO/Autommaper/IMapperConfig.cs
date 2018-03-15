using AutoMapper;
using System;
using System.Linq.Expressions;

namespace Api.Dto.Autommaper
{
    public interface IMap<TDestination, TSource> where TDestination : class where TSource : class
    {
        IMappingExpression<TSource, TDestination> Expression { get; }
        IMap<TDestination, TSource> Map<TDestinationProperty, TSourceProperty>(Expression<Func<TDestination, TDestinationProperty>> destinationProperty,
            Expression<Func<TSource, TSourceProperty>> sourceProperty);
    }
}