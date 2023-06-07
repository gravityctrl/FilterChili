using System.Numerics;
using SecondGeneration.Features.Descriptors;
using SecondGeneration.Features.Resolvers.Interfaces;
using SecondGeneration.Features.Resolvers.MultiValueResolvers;
using SecondGeneration.Features.Resolvers.SingleValueResolvers;
using SecondGeneration.Models.Enums;

namespace SecondGeneration.Features.Resolvers.FilterFactories;

internal class ComparisonFilterFactory<TSource, TValue> : FilterFactory<TSource, TValue> where TValue : IMinMaxValue<TValue>, IComparisonOperators<TValue, TValue, bool>
{
    private readonly FilterSettings _settings;
    private readonly Comparison _comparison;

    public ComparisonFilterFactory(FilterSettings settings, Comparison comparison)
    {
        _settings = settings;
        _comparison = comparison;
    }

    public override IFilterResolver<TSource, TValue> CreateFilterResolver(SingleFilterProperty<TSource, TValue> property) 
        => new SingleComparisonFilterResolver<TSource, TValue>(_settings, property, _comparison);

    public override IFilterResolver<TSource, TValue> CreateFilterResolver(MultiFilterProperty<TSource, TValue> property) 
        => new MultiComparisonFilterResolver<TSource, TValue>(_settings, property, _comparison);
}