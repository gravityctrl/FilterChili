namespace SecondGeneration.Features.Runtime;

internal class FilterContext<TSource> : IFilterContext<TSource>
{
    private readonly IQueryable<TSource> _queryable;
    private readonly IReadOnlyCollection<IResolver<TSource>> _resolvers;

    public FilterContext(IQueryable<TSource> queryable, IReadOnlyCollection<IResolver<TSource>> resolvers)
    {
        _queryable = queryable;
        _resolvers = resolvers;
    }

    public IQueryable<TSource> Filter()
    {
        return Apply(_resolvers);
    }

    public async Task<IEnumerable<object>> Inspect()
    {
        return await _resolvers
            .Select(resolver =>
            {
                var remainingResolvers = _resolvers.Where(item => item != resolver);
                var queryable = Apply(remainingResolvers);
                return resolver.BuildStatistics(_queryable, queryable);
            })
            .WhenAll();
    }

    private IQueryable<TSource> Apply(IEnumerable<IResolver<TSource>> resolvers)
    {
        return resolvers
            .Select(resolver => resolver.Predicate)
            .Aggregate(_queryable, (updatedQueryable, predicate) => updatedQueryable.Where(predicate));
    }
}