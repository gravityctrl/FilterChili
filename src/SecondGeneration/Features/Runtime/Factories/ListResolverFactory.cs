using Newtonsoft.Json.Linq;
using SecondGeneration.Features.Resolvers.Interfaces;
using SecondGeneration.Features.Runtime.Operators;
using SecondGeneration.Models.Filters;

namespace SecondGeneration.Features.Runtime.Factories;

internal class ListResolverFactory<TSource, TValue> : IResolverFactory<TSource>
{
    private readonly IListFilterResolver<TSource, TValue> _resolver;

    public string Name => _resolver.Name;
    
    public ListResolverFactory(IListFilterResolver<TSource, TValue> resolver)
    {
        _resolver = resolver;
    }

    public IResolver<TSource> Build()
    {
        return new ListResolver<TSource,TValue>(_resolver, Option.None());
    }

    public IResolver<TSource> Build(JToken filter)
    {
        var concreteFilter = Option.Some(filter.ToObject<ListFilter<TValue>>());
        return new ListResolver<TSource, TValue>(_resolver, concreteFilter);
    }
}