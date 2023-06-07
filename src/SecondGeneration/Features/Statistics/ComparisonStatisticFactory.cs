using Microsoft.EntityFrameworkCore;
using SecondGeneration.Features.Resolvers.Interfaces;
using SecondGeneration.Models.Filters;
using SecondGeneration.Models.Statistics;

namespace SecondGeneration.Features.Statistics;

internal class ComparisonStatisticFactory<TValue>
{
    private readonly IFilterResolver _resolver;
    
    public ComparisonStatisticFactory(IFilterResolver resolver)
    {
        _resolver = resolver;
    }

    public async Task<ComparisonStatistic<TValue>> Generate(IQueryable<TValue> available, IQueryable<TValue> selectable, Option<ComparisonFilter<TValue>> filter)
    {
        return new
        (
            _resolver.Name,
            await SetRange(available),
            await SetRange(selectable),
            filter.TryGetValue(out var content) 
                ? content.Value
                : default
        );
    }

    private static async Task<Range<TValue>?> SetRange(IQueryable<TValue> queryable)
    {
        // ReSharper disable once MethodHasAsyncOverload
        return queryable is IAsyncEnumerable<TValue>
            ? await ResolveRangeAsync(queryable)
            : ResolveRange(queryable);
    }

    private static Range<TValue>? ResolveRange(IQueryable<TValue> queryable)
    {
        if (!queryable.Any())
        {
            return null;
        }

        var min = queryable.Min();
        var max = queryable.Max();
        return new(min!, max!);
    }

    private static async Task<Range<TValue>?> ResolveRangeAsync(IQueryable<TValue> queryable)
    {
        if (!await queryable.AnyAsync())
        {
            return null;
        }

        var min = await queryable.MinAsync();
        var max = await queryable.MaxAsync();
        return new(min!, max!);
    }
}