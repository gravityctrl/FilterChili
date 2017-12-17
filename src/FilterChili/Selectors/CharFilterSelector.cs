using System;
using System.Linq.Expressions;
using GravityCTRL.FilterChili.Providers;

namespace GravityCTRL.FilterChili.Selectors
{
    public class CharFilterSelector<TSource> : FilterSelector<TSource, char, CharDomainProvider<TSource>>
    {
        internal CharFilterSelector(Expression<Func<TSource, char>> valueSelector) : base(new CharDomainProvider<TSource>(valueSelector)) {}
    }
}