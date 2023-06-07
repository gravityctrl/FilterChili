using Newtonsoft.Json.Linq;
using SecondGeneration.Features.Resolvers.Interfaces;
using SecondGeneration.Features.Runtime.Operators;
using SecondGeneration.Models.Filters;

namespace SecondGeneration.Features.Runtime.Factories;

internal class RangeResolverFactory<TSource, TValue> : IResolverFactory<TSource>
{
    private readonly IRangeFilterResolver<TSource, TValue> _resolver;

    public string Name => _resolver.Name;
    
    public RangeResolverFactory(IRangeFilterResolver<TSource, TValue> resolver)
    {
        _resolver = resolver;
    }

    public IResolver<TSource> Build()
    {
        return new RangeResolver<TSource,TValue>(_resolver, Option.None());
    }

    public IResolver<TSource> Build(JToken filter)
    {
        var concreteFilter = Option.Some(filter.ToObject<RangeFilter<TValue>>());
        return new RangeResolver<TSource, TValue>(_resolver, concreteFilter);
    }
}