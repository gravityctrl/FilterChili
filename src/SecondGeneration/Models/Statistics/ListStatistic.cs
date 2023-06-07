namespace SecondGeneration.Models.Statistics;

public class ListStatistic<TValue>
{
    public string FilterType => "List";
    
    public string Name { get; }
    public IEnumerable<ValueStatistic<TValue>> Values { get; }
    
    public ListStatistic(string name, IEnumerable<ValueStatistic<TValue>> values)
    {
        Name = name;
        Values = values;
    }
}