using System.Numerics;
using Microsoft.Extensions.DependencyInjection;
using SecondGeneration.Features.Configuration.Filter;
using SecondGeneration.Features.Descriptors;

namespace SecondGeneration.Features.Configuration;

internal class EntityConfigurator<TSource> : IEntityConfigurator<TSource>
{
    private readonly IServiceCollection _serviceCollection;
    
    public EntityConfigurator(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }

    public IElementFilterConfigurator Property(Expression<Func<TSource, string>> selector)
    {
        var property = new SingleFilterProperty<TSource, string>(selector);
        return new ElementFilterConfigurator<TSource, string>(_serviceCollection, property);
    }

    public INumberFilterConfigurator Property<TValue>(Expression<Func<TSource, TValue>> selector) where TValue : INumber<TValue>, IMinMaxValue<TValue>
    {
        var property = new SingleFilterProperty<TSource, TValue>(selector);
        return new NumberFilterConfigurator<TSource, TValue>(_serviceCollection, property);
    }

    public IMatchTypeConfigurator<IElementFilterConfigurator> Property(Expression<Func<TSource, IEnumerable<string>>> selector)
    {
        return MatchTypeConfigurator.Create(matchType =>
        {
            var property = new MultiFilterProperty<TSource, string>(matchType, selector);
            return new ElementFilterConfigurator<TSource, string>(_serviceCollection, property);
        });
    }

    public IMatchTypeConfigurator<INumberFilterConfigurator> Property<TValue>(Expression<Func<TSource, IEnumerable<TValue>>> selector) where TValue : INumber<TValue>, IMinMaxValue<TValue>
    {
        return MatchTypeConfigurator.Create(matchType =>
        {
            var property = new MultiFilterProperty<TSource, TValue>(matchType, selector);
            return new NumberFilterConfigurator<TSource, TValue>(_serviceCollection, property);
        });
    }
}