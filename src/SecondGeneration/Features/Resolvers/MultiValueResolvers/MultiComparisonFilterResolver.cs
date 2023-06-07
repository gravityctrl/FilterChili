using System.Numerics;
using SecondGeneration.Features.Descriptors;
using SecondGeneration.Features.Resolvers.ExpressionFactories;
using SecondGeneration.Features.Resolvers.Interfaces;
using SecondGeneration.Features.Statistics;
using SecondGeneration.Models.Enums;
using SecondGeneration.Models.Filters;
using SecondGeneration.Models.Statistics;

namespace SecondGeneration.Features.Resolvers.MultiValueResolvers;

internal class MultiComparisonFilterResolver<TSource, TValue> : IComparisonFilterResolver<TSource, TValue> 
    where TValue : IMinMaxValue<TValue>, IComparisonOperators<TValue, TValue, bool>
{
    private readonly FilterSettings _settings;
    private readonly MultiFilterProperty<TSource, TValue> _filterProperty;
    private readonly EnumerableExpressionFactory<TSource, TValue> _enumerableExpressionFactory;
    private readonly ComparisonExpressionFactory<TValue> _comparisonExpressionFactory;
    private readonly ComparisonStatisticFactory<TValue> _statisticFactory;
    public string Name => _settings.Name ?? _filterProperty.Name;

    public MultiComparisonFilterResolver(FilterSettings settings, MultiFilterProperty<TSource, TValue> filterProperty, Comparison comparison)
    {
        _settings = settings;
        _filterProperty = filterProperty;

        var parameter = Expression.Parameter(typeof(TValue), "element");
        
        _enumerableExpressionFactory = new(filterProperty);
        _comparisonExpressionFactory = new(comparison, parameter, parameter);
        _statisticFactory = new(this);
    }

    public Option<Expression<Func<TSource, bool>>> BuildPredicate(ComparisonFilter<TValue> filter)
    {
        var expression = _comparisonExpressionFactory.Create<TValue>(filter);
        return _enumerableExpressionFactory.Create(expression);
    }

    public Task<ComparisonStatistic<TValue>> BuildStatistics(IQueryable<TSource> available, IQueryable<TSource> selectable, Option<ComparisonFilter<TValue>> filter)
    {
        var availableValues = _filterProperty.GetValues(available);
        var selectableValues = _filterProperty.GetValues(selectable);
        return _statisticFactory.Generate(availableValues, selectableValues, filter);
    }
}