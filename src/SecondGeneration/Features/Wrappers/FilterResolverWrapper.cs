using SecondGeneration.Features.Resolvers.Interfaces;
using SecondGeneration.Features.Runtime;
using SecondGeneration.Features.Runtime.Factories;

namespace SecondGeneration.Features.Wrappers;

internal static class FilterResolverWrapper
{
    public static IResolverFactory<TSource> Wrap<TSource, TValue>(IFilterResolver<TSource, TValue> filterResolver)
    {
        return filterResolver switch
        {
            IListFilterResolver<TSource, TValue> resolver => new ListResolverFactory<TSource,TValue>(resolver),
            IRangeFilterResolver<TSource, TValue> resolver => new RangeResolverFactory<TSource,TValue>(resolver),
            IComparisonFilterResolver<TSource, TValue> resolver => new ComparisonResolverFactory<TSource,TValue>(resolver),
            _ => throw new ArgumentOutOfRangeException(nameof(filterResolver), filterResolver, default)
        };
    }
}