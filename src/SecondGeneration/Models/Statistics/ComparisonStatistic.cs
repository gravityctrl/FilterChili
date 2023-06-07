namespace SecondGeneration.Models.Statistics;

public class ComparisonStatistic<TValue>
{
    public string FilterType => "Comparison";
    
    public string Name { get; }
    public Range<TValue>? TotalRange { get; }
    public Range<TValue>? SelectableRange { get; }
    public TValue? SelectedValue { get; }
    
    public ComparisonStatistic(string name, Range<TValue>? totalRange, Range<TValue>? selectableRange, TValue? selectedValue)
    {
        Name = name;
        TotalRange = totalRange;
        SelectableRange = selectableRange;
        SelectedValue = selectedValue;
    }
}