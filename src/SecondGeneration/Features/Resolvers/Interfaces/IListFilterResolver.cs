using SecondGeneration.Models.Filters;
using SecondGeneration.Models.Statistics;

namespace SecondGeneration.Features.Resolvers.Interfaces;

internal interface IListFilterResolver<TSource, TValue> : IFilterResolver<TSource, TValue>
{
    Option<Expression<Func<TSource, bool>>> BuildPredicate(ListFilter<TValue> filter);
    Task<ListStatistic<TValue>> BuildStatistics(IQueryable<TSource> available, IQueryable<TSource> selectable, Option<ListFilter<TValue>> filter);
}