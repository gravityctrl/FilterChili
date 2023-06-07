using SecondGeneration.Features.Resolvers.FilterFactories;
using SecondGeneration.Features.Resolvers.Interfaces;
using MatchType = SecondGeneration.Models.Enums.MatchType;

namespace SecondGeneration.Features.Descriptors;

internal class MultiFilterProperty<TSource, TValue> : IFilterProperty<TSource, TValue>
{
    private readonly Expression<Func<TSource, IEnumerable<TValue>>> _selector;

    public string Name => _selector.Name();
    public MatchType MatchType { get; }
    public LambdaExpression Selector => _selector;

    public MultiFilterProperty(MatchType matchType, Expression<Func<TSource, IEnumerable<TValue>>> selector)
    {
        _selector = selector;
        MatchType = matchType;
    }

    public IQueryable<TValue> GetValues(IQueryable<TSource> source)
    {
        return source.SelectMany(_selector);
    }
    
    public IFilterResolver<TSource, TValue> CreateFilterResolver(FilterFactory<TSource, TValue> filterFactory)
    {
        return filterFactory.CreateFilterResolver(this);
    }
}