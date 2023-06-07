namespace SecondGeneration.Models.Filters;

public class ListFilter<TValue>
{
    public IReadOnlyCollection<TValue> Values { get; }

    public ListFilter(IReadOnlyCollection<TValue> values)
    {
        Values = values;
    }
}