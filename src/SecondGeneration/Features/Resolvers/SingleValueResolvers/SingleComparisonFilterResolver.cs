using System.Numerics;
using SecondGeneration.Features.Descriptors;
using SecondGeneration.Features.Resolvers.ExpressionFactories;
using SecondGeneration.Features.Resolvers.Interfaces;
using SecondGeneration.Features.Statistics;
using SecondGeneration.Models.Enums;
using SecondGeneration.Models.Filters;
using SecondGeneration.Models.Statistics;

namespace SecondGeneration.Features.Resolvers.SingleValueResolvers;

internal class SingleComparisonFilterResolver<TSource, TValue> : IComparisonFilterResolver<TSource, TValue> 
    where TValue : IMinMaxValue<TValue>, IComparisonOperators<TValue, TValue, bool>
{
    private readonly FilterSettings _settings;
    private readonly SingleFilterProperty<TSource, TValue> _filterProperty;
    private readonly ComparisonExpressionFactory<TValue> _expressionFactory;
    private readonly ComparisonStatisticFactory<TValue> _statisticFactory;
    public string Name => _settings.Name ?? _filterProperty.Name;

    public SingleComparisonFilterResolver(FilterSettings settings, SingleFilterProperty<TSource, TValue> filterProperty, Comparison comparison)
    {
        _settings = settings;
        _filterProperty = filterProperty;
        _expressionFactory = new(comparison, filterProperty.Selector.Body, filterProperty.Selector.Parameters.ToArray());
        _statisticFactory = new(this);
    }

    public Option<Expression<Func<TSource, bool>>> BuildPredicate(ComparisonFilter<TValue> filter)
    {
        return _expressionFactory.Create<TSource>(filter);
    }

    public Task<ComparisonStatistic<TValue>> BuildStatistics(IQueryable<TSource> available, IQueryable<TSource> selectable, Option<ComparisonFilter<TValue>> filter)
    {
        var availableValues = _filterProperty.GetValues(available);
        var selectableValues = _filterProperty.GetValues(selectable);
        return _statisticFactory.Generate(availableValues, selectableValues, filter);
    }
}