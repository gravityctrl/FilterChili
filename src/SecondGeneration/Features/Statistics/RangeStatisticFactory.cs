using Microsoft.EntityFrameworkCore;
using SecondGeneration.Features.Resolvers.Interfaces;
using SecondGeneration.Models.Filters;
using SecondGeneration.Models.Statistics;

namespace SecondGeneration.Features.Statistics;

internal class RangeStatisticFactory<TValue>
{
    private readonly IFilterResolver _resolver;
    
    public RangeStatisticFactory(IFilterResolver resolver)
    {
        _resolver = resolver;
    }

    public async Task<RangeStatistic<TValue>> Generate(IQueryable<TValue> available, IQueryable<TValue> selectable, Option<RangeFilter<TValue>> filter)
    {
        var totalRange = await SetRange(available);
        return new
        (
            _resolver.Name,
            totalRange,
            await SetRange(selectable),
            filter.TryGetValue(out var content) 
                ? new(content.Min, content.Max)
                : totalRange
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