using Microsoft.EntityFrameworkCore;
using SecondGeneration.Extensions;
using SecondGeneration.Features.Resolvers.Interfaces;
using SecondGeneration.Models.Filters;
using SecondGeneration.Models.Statistics;

namespace SecondGeneration.Features.Statistics;

internal class ListStatisticFactory<TValue> where TValue : notnull
{
    private readonly IFilterResolver _resolver;

    public ListStatisticFactory(IFilterResolver resolver)
    {
        _resolver = resolver;
    }

    public async Task<ListStatistic<TValue>> Generate(IQueryable<TValue> available, IQueryable<TValue> selectable, Option<ListFilter<TValue>> filter)
    {
        var availableItems = await CreateSelectorList(available);
        var selectableItems = await CreateSelectorList(selectable);
        var entities = availableItems.ToDictionary(value => value, value => new ValueStatistic<TValue>(value));
        
        selectableItems.ForEach(value =>
        {
            if (entities.TryGetValue(value, out var entity))
            {
                entity.CanBeSelected = true;
            }
        });
        
        if (filter.TryGetValue(out var content))
        {
            content.Values.ForEach(value =>
            {
                if (entities.TryGetValue(value, out var entity))
                {
                    entity.IsSelected = true;
                }
            });
        }

        return new(_resolver.Name, entities.Values);
    }

    private static async Task<IReadOnlyCollection<TValue>> CreateSelectorList(IQueryable<TValue> queryable)
    {
        return queryable is IAsyncEnumerable<TValue>
            ? await queryable.Distinct().ToListAsync()
            : queryable.Distinct().ToList();
    }
}