using SecondGeneration.Features.Descriptors;
using SecondGeneration.Features.Resolvers.ExpressionFactories;
using SecondGeneration.Features.Resolvers.Interfaces;
using SecondGeneration.Features.Statistics;
using SecondGeneration.Models.Filters;
using SecondGeneration.Models.Statistics;

namespace SecondGeneration.Features.Resolvers.MultiValueResolvers;

internal class MultiListFilterResolver<TSource, TValue> : IListFilterResolver<TSource, TValue> 
    where TValue : notnull
{
    private readonly FilterSettings _settings;
    private readonly MultiFilterProperty<TSource, TValue> _filterProperty;
    private readonly EnumerableExpressionFactory<TSource, TValue> _enumerableExpressionFactory;
    private readonly ListExpressionFactory<TValue> _listExpressionFactory;
    private readonly ListStatisticFactory<TValue> _statisticFactory;
    public string Name => _settings.Name ?? _filterProperty.Name;

    public MultiListFilterResolver(FilterSettings settings, MultiFilterProperty<TSource, TValue> filterProperty)
    {
        _settings = settings;
        _filterProperty = filterProperty;

        var parameter = Expression.Parameter(typeof(TValue), "item");
        
        _enumerableExpressionFactory = new(filterProperty);
        _listExpressionFactory = new(parameter, parameter);
        _statisticFactory = new(this);
    }

    public Option<Expression<Func<TSource, bool>>> BuildPredicate(ListFilter<TValue> filter)
    {
        var expression = _listExpressionFactory.Create<TValue>(filter);
        return _enumerableExpressionFactory.Create(expression);
    }

    public Task<ListStatistic<TValue>> BuildStatistics(IQueryable<TSource> available, IQueryable<TSource> selectable, Option<ListFilter<TValue>> filter)
    {
        var availableValues = _filterProperty.GetValues(available);
        var selectableValues = _filterProperty.GetValues(selectable);
        return _statisticFactory.Generate(availableValues, selectableValues, filter);
    }
}