using SecondGeneration.Features.Descriptors;
using SecondGeneration.Features.Resolvers.Interfaces;

namespace SecondGeneration.Features.Resolvers.FilterFactories;

internal abstract class FilterFactory<TSource, TValue>
{
    public abstract IFilterResolver<TSource, TValue> CreateFilterResolver(SingleFilterProperty<TSource, TValue> property);
    public abstract IFilterResolver<TSource, TValue> CreateFilterResolver(MultiFilterProperty<TSource, TValue> property);
}