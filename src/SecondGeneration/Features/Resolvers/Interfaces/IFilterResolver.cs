namespace SecondGeneration.Features.Resolvers.Interfaces;

internal interface IFilterResolver
{
    string Name { get; }
}

// ReSharper disable UnusedTypeParameter
internal interface IFilterResolver<TSource, TValue> : IFilterResolver {}