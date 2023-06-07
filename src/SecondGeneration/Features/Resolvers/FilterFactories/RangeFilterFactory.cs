using System.Numerics;
using SecondGeneration.Features.Descriptors;
using SecondGeneration.Features.Resolvers.Interfaces;
using SecondGeneration.Features.Resolvers.MultiValueResolvers;
using SecondGeneration.Features.Resolvers.SingleValueResolvers;

namespace SecondGeneration.Features.Resolvers.FilterFactories;

internal class RangeFilterFactory<TSource, TValue> : FilterFactory<TSource, TValue> where TValue : IMinMaxValue<TValue>, IComparisonOperators<TValue, TValue, bool>
{
    private readonly FilterSettings _settings;

    public RangeFilterFactory(FilterSettings settings)
    {
        _settings = settings;
    }

    public override IFilterResolver<TSource, TValue> CreateFilterResolver(SingleFilterProperty<TSource, TValue> property) 
        => new SingleRangeFilterResolver<TSource, TValue>(_settings, property);

    public override IFilterResolver<TSource, TValue> CreateFilterResolver(MultiFilterProperty<TSource, TValue> property) 
        => new MultiRangeFilterResolver<TSource, TValue>(_settings, property);
}