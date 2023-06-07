namespace SecondGeneration;

public interface INumberFilterConfigurator : IElementFilterConfigurator
{
    void WithLessThan(Action<FilterSettings>? configure = null);
    void WithGreaterThan(Action<FilterSettings>? configure = null);
    void WithLessThanOrEqual(Action<FilterSettings>? configure = null);
    void WithGreaterThanOrEqual(Action<FilterSettings>? configure = null);
    void WithRange(Action<FilterSettings>? configure = null);
}