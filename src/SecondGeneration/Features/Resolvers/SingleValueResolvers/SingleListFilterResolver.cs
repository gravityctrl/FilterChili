using SecondGeneration.Features.Descriptors;
using SecondGeneration.Features.Resolvers.ExpressionFactories;
using SecondGeneration.Features.Resolvers.Interfaces;
using SecondGeneration.Features.Statistics;
using SecondGeneration.Models.Filters;
using SecondGeneration.Models.Statistics;

namespace SecondGeneration.Features.Resolvers.SingleValueResolvers;

internal class SingleListFilterResolver<TSource, TValue> : IListFilterResolver<TSource, TValue> 
    where TValue : notnull
{
    private readonly FilterSettings _settings;
    private readonly SingleFilterProperty<TSource, TValue> _filterProperty;
    private readonly ListExpressionFactory<TValue> _expressionFactory;
    private readonly ListStatisticFactory<TValue> _statisticFactory;
    public string Name => _settings.Name ?? _filterProperty.Name;

    public SingleListFilterResolver(FilterSettings settings, SingleFilterProperty<TSource, TValue> filterProperty)
    {
        _settings = settings;
        _filterProperty = filterProperty;
        _expressionFactory = new(filterProperty.Selector.Body, filterProperty.Selector.Parameters.ToArray());
        _statisticFactory = new(this);
    }

    public Option<Expression<Func<TSource, bool>>> BuildPredicate(ListFilter<TValue> filter)
    {
        return _expressionFactory.Create<TSource>(filter);
    }

    public Task<ListStatistic<TValue>> BuildStatistics(IQueryable<TSource> available, IQueryable<TSource> selectable, Option<ListFilter<TValue>> filter)
    {
        var availableValues = _filterProperty.GetValues(available);
        var selectableValues = _filterProperty.GetValues(selectable);
        return _statisticFactory.Generate(availableValues, selectableValues, filter);
    }
}