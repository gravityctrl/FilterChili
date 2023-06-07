using Microsoft.Extensions.DependencyInjection;
using SecondGeneration.Features.Configuration;
using SecondGeneration.Features.Runtime;

namespace SecondGeneration;

public static class DependencyInjection
{
    public static void AddFilter<TSource>(this IServiceCollection serviceCollection, Action<IEntityConfigurator<TSource>> configure)
    {
        var options = new EntityConfigurator<TSource>(serviceCollection);
        configure(options);
        serviceCollection.AddScoped<IFilter<TSource>, Filter<TSource>>();
    }
}