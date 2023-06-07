using Microsoft.Extensions.DependencyInjection;
using SecondGeneration.Features.Descriptors;
using SecondGeneration.Features.Resolvers.FilterFactories;

namespace SecondGeneration.Features.Configuration.Filter;

internal class ElementFilterConfigurator<TSource, TValue> : FilterConfigurator<TSource, TValue>, IElementFilterConfigurator 
    where TValue : notnull
{
    public ElementFilterConfigurator(IServiceCollection collection, IFilterProperty<TSource, TValue> property) 
        : base(collection, property) {}

    public void WithList(Action<FilterSettings>? configure = null)
    {
        var settings = new FilterSettings();
        configure?.Invoke(settings);

        var factory = new ListFilterFactory<TSource, TValue>(settings);
        Register(factory);
    }
}