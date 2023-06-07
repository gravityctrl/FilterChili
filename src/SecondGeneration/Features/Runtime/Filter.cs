namespace SecondGeneration.Features.Runtime;

internal class Filter<TSource> : IFilter<TSource>
{
    private readonly IEnumerable<IResolverFactory<TSource>> _resolverFactories;
    
    public Filter(IEnumerable<IResolverFactory<TSource>> resolverFactories)
    {
        _resolverFactories = resolverFactories;
    }
    
    public IFilterContext<TSource> Build(IQueryable<TSource> queryable, IEnumerable<NamedFilter> filterModels)
    {
        var indexedFilters = filterModels.ToDictionary
        (
            model => model.Name, 
            model => model.Filter
        );

        IResolver<TSource> BuildResolver(IResolverFactory<TSource> resolverFactory)
        {
            return indexedFilters.TryGetValue(resolverFactory.Name, out var filter)
                ? resolverFactory.Build(filter)
                : resolverFactory.Build();
        }

        var resolvers = _resolverFactories
            .Select(BuildResolver)
            .ToList();

        return new FilterContext<TSource>(queryable, resolvers);
    }
}