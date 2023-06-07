namespace SecondGeneration.Models.Filters;

public class ComparisonFilter<TValue>
{
    public TValue Value { get; }

    public ComparisonFilter(TValue value)
    {
        Value = value;
    }
}