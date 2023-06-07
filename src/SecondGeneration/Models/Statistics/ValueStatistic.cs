namespace SecondGeneration.Models.Statistics;

public class ValueStatistic<TValue>
{
    public TValue Value { get; }
    public bool IsSelected { get; internal set; }
    public bool CanBeSelected { get; internal set; }
    
    public ValueStatistic(TValue value)
    {
        Value = value;
    }
}