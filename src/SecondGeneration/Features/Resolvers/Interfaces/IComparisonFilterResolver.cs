using SecondGeneration.Models.Filters;
using SecondGeneration.Models.Statistics;

namespace SecondGeneration.Features.Resolvers.Interfaces;

internal interface IComparisonFilterResolver<TSource, TValue> : IFilterResolver<TSource, TValue>
{
    Option<Expression<Func<TSource, bool>>> BuildPredicate(ComparisonFilter<TValue> filter);
    Task<ComparisonStatistic<TValue>> BuildStatistics(IQueryable<TSource> available, IQueryable<TSource> selectable, Option<ComparisonFilter<TValue>> filter);
}