using Newtonsoft.Json.Linq;

namespace SecondGeneration.Features.Runtime;

internal interface IResolverFactory<TSource>
{
    string Name { get; }
    IResolver<TSource> Build();
    IResolver<TSource> Build(JToken filter);
}