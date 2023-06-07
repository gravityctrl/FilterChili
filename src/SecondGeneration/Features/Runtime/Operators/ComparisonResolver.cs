using SecondGeneration.Features.Resolvers.Interfaces;
using SecondGeneration.Models.Filters;

namespace SecondGeneration.Features.Runtime.Operators;

internal class ComparisonResolver<TSource, TValue> : IResolver<TSource>
{
    private readonly IComparisonFilterResolver<TSource, TValue> _resolver;
    private readonly Option<ComparisonFilter<TValue>> _filter;
    
    public Option<Expression<Func<TSource, bool>>> Predicate { get; }
    
    public ComparisonResolver(IComparisonFilterResolver<TSource, TValue> resolver, Option<ComparisonFilter<TValue>> filter)
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