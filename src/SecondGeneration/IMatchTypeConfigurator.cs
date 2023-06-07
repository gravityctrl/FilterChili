namespace SecondGeneration;

public interface IMatchTypeConfigurator<out TFilterConfigurator>
{
    TFilterConfigurator Any { get; }
    TFilterConfigurator All { get; }
}