using GravityCTRL.FilterChili.Resolvers;
using JetBrains.Annotations;

namespace GravityCTRL.FilterChili.Extensions
{
    public static class DomainResolverExtensions
    {
        [UsedImplicitly]
        public static TDomainResolver UseName<TDomainResolver>(this TDomainResolver resolver, string name) where TDomainResolver : DomainResolver
        {
            resolver.Name = name;
            return resolver;
        }
    }
}