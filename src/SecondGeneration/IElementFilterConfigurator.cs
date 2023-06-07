namespace SecondGeneration;

public interface IElementFilterConfigurator
{
    void WithList(Action<FilterSettings>? configure = null);
}