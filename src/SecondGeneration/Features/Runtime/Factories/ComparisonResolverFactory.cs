using Newtonsoft.Json.Linq;
using SecondGeneration.Features.Resolvers.Interfaces;
using SecondGeneration.Features.Runtime.Operators;
using SecondGeneration.Models.Filters;

namespace SecondGeneration.Features.Runtime.Factories;

internal class ComparisonResolverFactory<TSource, TValue> : IResolverFactory<TSource>
{
    private readonly IComparisonFilterResolver<TSource, TValue> _resolver;

    public string Name => _resolver.Name;
    
    public ComparisonResolverFactory(IComparisonFilterResolver<TSource, TValue> resolver)
    {
        _resolver = resolver;
    }

    public IResolver<TSource> Build()
    {
        return new ComparisonResolver<TSource, TValue>(_resolver, Option.None());
    }

    public IResolver<TSource> Build(JToken filter)
    {
        var concreteFilter = Option.Some(filter.ToObject<ComparisonFilter<TValue>>());
        return new ComparisonResolver<TSource, TValue>(_resolver, concreteFilter);
    }
}