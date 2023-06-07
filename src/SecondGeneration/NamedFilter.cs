using Newtonsoft.Json.Linq;

namespace SecondGeneration;

public class NamedFilter
{
    public string Name { get; }
    public JToken Filter { get; }

    public NamedFilter(string name, JToken filter)
    {
        Name = name;
        Filter = filter;
    }
}