namespace SecondGeneration.Models.Statistics;

public class RangeStatistic<TValue>
{
    public string FilterType => "Range";
    
    public string Name { get; }
    public Range<TValue>? TotalRange { get; }
    public Range<TValue>? SelectableRange { get; }
    public Range<TValue>? SelectedRange { get; }
    
    public RangeStatistic(string name, Range<TValue>? totalRange, Range<TValue>? selectableRange, Range<TValue>? selectedRange)
    {
        Name = name;
        TotalRange = totalRange;
        SelectableRange = selectableRange;
        SelectedRange = selectedRange;
    }
}