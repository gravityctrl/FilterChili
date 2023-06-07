using System.Numerics;
using SecondGeneration.Features.Descriptors;
using SecondGeneration.Features.Resolvers.ExpressionFactories;
using SecondGeneration.Features.Resolvers.Interfaces;
using SecondGeneration.Features.Statistics;
using SecondGeneration.Models.Filters;
using SecondGeneration.Models.Statistics;

namespace SecondGeneration.Features.Resolvers.MultiValueResolvers;

internal class MultiRangeFilterResolver<TSource, TValue> : IRangeFilterResolver<TSource, TValue> 
    where TValue : IMinMaxValue<TValue>, IComparisonOperators<TValue, TValue, bool>
{
    private readonly FilterSettings _settings;
    private readonly MultiFilterProperty<TSource, TValue> _filterProperty;
    private readonly EnumerableExpressionFactory<TSource, TValue> _enumerableExpressionFactory;
    private readonly RangeExpressionFactory<TValue> _rangeExpressionFactory;
    private readonly RangeStatisticFactory<TValue> _statisticFactory;
    public string Name => _settings.Name ?? _filterProperty.Name;

    public MultiRangeFilterResolver(FilterSettings settings, MultiFilterProperty<TSource, TValue> filterProperty)
    {
        _settings = settings;
        _filterProperty = filterProperty;

        var parameter = Expression.Parameter(typeof(TValue), "element");
        
        _enumerableExpressionFactory = new(filterProperty);
        _rangeExpressionFactory = new(parameter, parameter);
        _statisticFactory = new(this);
    }

    public Option<Expression<Func<TSource, bool>>> BuildPredicate(RangeFilter<TValue> filter)
    {
        var expression = _rangeExpressionFactory.Create<TValue>(filter);
        return _enumerableExpressionFactory.Create(expression);
    }

    public Task<RangeStatistic<TValue>> BuildStatistics(IQueryable<TSource> available, IQueryable<TSource> selectable, Option<RangeFilter<TValue>> filter)
    {
        var availableValues = _filterProperty.GetValues(available);
        var selectableValues = _filterProperty.GetValues(selectable);
        return _statisticFactory.Generate(availableValues, selectableValues, filter);
    }
}