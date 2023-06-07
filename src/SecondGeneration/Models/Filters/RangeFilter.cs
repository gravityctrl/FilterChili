namespace SecondGeneration.Models.Filters;

public class RangeFilter<TValue>
{
    public TValue Min { get; }
    public TValue Max { get; }
    
    public RangeFilter(TValue min, TValue max)
    {
        Min = min;
        Max = max;
    }
}