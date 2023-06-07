using System.Numerics;
using Microsoft.Extensions.DependencyInjection;
using SecondGeneration.Features.Descriptors;
using SecondGeneration.Features.Resolvers.FilterFactories;
using SecondGeneration.Models.Enums;

namespace SecondGeneration.Features.Configuration.Filter;

internal class NumberFilterConfigurator<TSource, TValue> : ElementFilterConfigurator<TSource, TValue>, INumberFilterConfigurator
    where TValue : IMinMaxValue<TValue>, IComparisonOperators<TValue, TValue, bool>
{
    public NumberFilterConfigurator(IServiceCollection collection, IFilterProperty<TSource, TValue> property) 
        : base(collection, property) {}

    public void WithLessThan(Action<FilterSettings>? configure = null)
    {
        RegisterComparisonFilter(Comparison.LessThan, configure);
    }

    public void WithGreaterThan(Action<FilterSettings>? configure = null)
    {
        RegisterComparisonFilter(Comparison.GreaterThan, configure);
    }

    public void WithLessThanOrEqual(Action<FilterSettings>? configure = null)
    {
        RegisterComparisonFilter(Comparison.LessThanOrEqual, configure);
    }

    public void WithGreaterThanOrEqual(Action<FilterSettings>? configure = null)
    {
        RegisterComparisonFilter(Comparison.GreaterThanOrEqual, configure);
    }

    public void WithRange(Action<FilterSettings>? configure = null)
    {
        var settings = new FilterSettings();
        configure?.Invoke(settings);

        var factory = new RangeFilterFactory<TSource, TValue>(settings);
        Register(factory);
    }

    private void RegisterComparisonFilter(Comparison comparison, Action<FilterSettings>? configure)
    {
        var settings = new FilterSettings();
        configure?.Invoke(settings);

        var factory = new ComparisonFilterFactory<TSource, TValue>(settings, comparison);
        Register(factory);
    }
}