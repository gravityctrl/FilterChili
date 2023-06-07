using SecondGeneration.Features.Resolvers.Interfaces;
using SecondGeneration.Models.Filters;

namespace SecondGeneration.Features.Runtime.Operators;

internal class RangeResolver<TSource, TValue> : IResolver<TSource>
{
    private readonly IRangeFilterResolver<TSource, TValue> _resolver;
    private readonly Option<RangeFilter<TValue>> _filter;
    
    public Option<Expression<Func<TSource, bool>>> Predicate { get; }
    
    public RangeResolver(IRangeFilterResolver<TSource, TValue> resolver, Option<RangeFilter<TValue>> filter)
    {
        _resolver = resolver;
        _filter = filter;
        Predicate = filter.Map(resolver.BuildPredicate);
    }

    public async Task<object> BuildStatistics(IQueryable<TSource> available, IQueryable<TSource> selectable)
    {
        return await _resolver.BuildStatistics(available, selectable, _filter);
    }
}