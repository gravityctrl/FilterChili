namespace SecondGeneration.Features.Runtime;

internal interface IResolver<TSource>
{
    Option<Expression<Func<TSource, bool>>> Predicate { get; }
    Task<object> BuildStatistics(IQueryable<TSource> available, IQueryable<TSource> selectable);
}