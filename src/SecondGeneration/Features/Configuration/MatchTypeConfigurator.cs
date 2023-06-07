using MatchType = SecondGeneration.Models.Enums.MatchType;

namespace SecondGeneration.Features.Configuration;

internal static class MatchTypeConfigurator
{
    internal static IMatchTypeConfigurator<TFilterConfigurator> Create<TFilterConfigurator>(Func<MatchType, TFilterConfigurator> setMatchType)
    {
        return new MatchTypeConfigurator<TFilterConfigurator>(setMatchType);
    }
}

internal class MatchTypeConfigurator<TFilterConfigurator> : IMatchTypeConfigurator<TFilterConfigurator>
{
    private readonly Func<MatchType, TFilterConfigurator> _setMatchType;

    public TFilterConfigurator Any => _setMatchType(MatchType.Any);
    public TFilterConfigurator All => _setMatchType(MatchType.All);
    
    public MatchTypeConfigurator(Func<MatchType, TFilterConfigurator> setMatchType)
    {
        _setMatchType = setMatchType;
    }
}