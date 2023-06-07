using SecondGeneration.Features.Resolvers.FilterFactories;
using SecondGeneration.Features.Resolvers.Interfaces;

namespace SecondGeneration.Features.Descriptors;

internal class SingleFilterProperty<TSource, TValue> : IFilterProperty<TSource, TValue>
{
    private readonly Expression<Func<TSource, TValue>> _selector;

    public string Name => _selector.Name();
    public LambdaExpression Selector => _selector;

    public SingleFilterProperty(Expression<Func<TSource, TValue>> selector)
    {
        _selector = selector;
    }

    public IQueryable<TValue> GetValues(IQueryable<TSource> source)
    {
        return source.Select(_selector);
    }
    
    public IFilterResolver<TSource, TValue> CreateFilterResolver(FilterFactory<TSource, TValue> filterFactory)
    {
        return filterFactory.CreateFilterResolver(this);
    }
}