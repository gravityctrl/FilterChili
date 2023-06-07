using SecondGeneration.Features.Resolvers.Interfaces;
using SecondGeneration.Models.Filters;

namespace SecondGeneration.Features.Runtime.Operators;

internal class ListResolver<TSource, TValue> : IResolver<TSource>
{
    private readonly IListFilterResolver<TSource, TValue> _resolver;
    private readonly Option<ListFilter<TValue>> _filter;
    
    public Option<Expression<Func<TSource, bool>>> Predicate { get; }
    
    public ListResolver(IListFilterResolver<TSource, TValue> resolver, Option<ListFilter<TValue>> filter)
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