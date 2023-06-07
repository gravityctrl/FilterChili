using SecondGeneration.Features.Resolvers.FilterFactories;
using SecondGeneration.Features.Resolvers.Interfaces;

namespace SecondGeneration.Features.Descriptors;

internal interface IFilterProperty<TSource, TValue>
{
    IFilterResolver<TSource, TValue> CreateFilterResolver(FilterFactory<TSource, TValue> filterFactory);
}