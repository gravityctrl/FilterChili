using Microsoft.Extensions.DependencyInjection;
using SecondGeneration.Features.Descriptors;
using SecondGeneration.Features.Resolvers.FilterFactories;
using SecondGeneration.Features.Wrappers;

namespace SecondGeneration.Features.Configuration.Filter;

internal abstract class FilterConfigurator<TSource, TValue>
{
    private readonly IServiceCollection _collection;
    private readonly IFilterProperty<TSource, TValue> _property;

    protected FilterConfigurator(IServiceCollection collection, IFilterProperty<TSource, TValue> property)
    {
        _collection = collection;
        _property = property;
    }

    protected void Register(FilterFactory<TSource, TValue> factory)
    {
        var filterResolver = _property.CreateFilterResolver(factory);
        _collection.AddSingleton(FilterResolverWrapper.Wrap(filterResolver));
    }
}