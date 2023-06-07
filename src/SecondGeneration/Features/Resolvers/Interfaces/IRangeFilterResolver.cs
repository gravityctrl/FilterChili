using SecondGeneration.Models.Filters;
using SecondGeneration.Models.Statistics;

namespace SecondGeneration.Features.Resolvers.Interfaces;

internal interface IRangeFilterResolver<TSource, TValue> : IFilterResolver<TSource, TValue>
{
    Option<Expression<Func<TSource, bool>>> BuildPredicate(RangeFilter<TValue> filter);
    Task<RangeStatistic<TValue>> BuildStatistics(IQueryable<TSource> available, IQueryable<TSource> selectable, Option<RangeFilter<TValue>> filter);
}