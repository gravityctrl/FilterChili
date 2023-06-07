namespace SecondGeneration.Models.Statistics;

public class Range<TValue>
{
    public TValue Min { get; }
    public TValue Max { get; }
    
    public Range(TValue min, TValue max)
    {
        Min = min;
        Max = max;
    }
}