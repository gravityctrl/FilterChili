using System.Numerics;
using SecondGeneration.Features.Descriptors;
using SecondGeneration.Features.Resolvers.ExpressionFactories;
using SecondGeneration.Features.Resolvers.Interfaces;
using SecondGeneration.Features.Statistics;
using SecondGeneration.Models.Filters;
using SecondGeneration.Models.Statistics;

namespace SecondGeneration.Features.Resolvers.SingleValueResolvers;

internal class SingleRangeFilterResolver<TSource, TValue> : IRangeFilterResolver<TSource, TValue> 
    where TValue : IMinMaxValue<TValue>, IComparisonOperators<TValue, TValue, bool>
{
    private readonly FilterSettings _settings;
    private readonly SingleFilterProperty<TSource, TValue> _filterProperty;
    private readonly RangeExpressionFactory<TValue> _expressionFactory;
    private readonly RangeStatisticFactory<TValue> _statisticFactory;
    public string Name => _settings.Name ?? _filterProperty.Name;

    public SingleRangeFilterResolver(FilterSettings settings, SingleFilterProperty<TSource, TValue> filterProperty)
    {
        _settings = settings;
        _filterProperty = filterProperty;
        _expressionFactory = new(filterProperty.Selector.Body, filterProperty.Selector.Parameters.ToArray());
        _statisticFactory = new(this);
    }

    public Option<Expression<Func<TSource, bool>>> BuildPredicate(RangeFilter<TValue> filter)
    {
        return _expressionFactory.Create<TSource>(filter);
    }

    public Task<RangeStatistic<TValue>> BuildStatistics(IQueryable<TSource> available, IQueryable<TSource> selectable, Option<RangeFilter<TValue>> filter)
    {
        var availableValues = _filterProperty.GetValues(available);
        var selectableValues = _filterProperty.GetValues(selectable);
        return _statisticFactory.Generate(availableValues, selectableValues, filter);
    }
}