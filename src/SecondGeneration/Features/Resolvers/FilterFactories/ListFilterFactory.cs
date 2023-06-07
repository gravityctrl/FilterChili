using SecondGeneration.Features.Descriptors;
using SecondGeneration.Features.Resolvers.Interfaces;
using SecondGeneration.Features.Resolvers.MultiValueResolvers;
using SecondGeneration.Features.Resolvers.SingleValueResolvers;

namespace SecondGeneration.Features.Resolvers.FilterFactories;

internal class ListFilterFactory<TSource, TValue> : FilterFactory<TSource, TValue> where TValue : notnull
{
    private readonly FilterSettings _settings;

    public ListFilterFactory(FilterSettings settings)
    {
        _settings = settings;
    }

    public override IFilterResolver<TSource, TValue> CreateFilterResolver(SingleFilterProperty<TSource, TValue> property) 
        => new SingleListFilterResolver<TSource, TValue>(_settings, property);

    public override IFilterResolver<TSource, TValue> CreateFilterResolver(MultiFilterProperty<TSource, TValue> property) 
        => new MultiListFilterResolver<TSource, TValue>(_settings, property);
}