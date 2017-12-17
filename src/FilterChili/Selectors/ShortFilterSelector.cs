using System;
using System.Linq.Expressions;
using GravityCTRL.FilterChili.Providers;

namespace GravityCTRL.FilterChili.Selectors
{
    public class ShortFilterSelector<TSource> : FilterSelector<TSource, short, ShortDomainProvider<TSource>>
    {
        internal ShortFilterSelector(Expression<Func<TSource, short>> valueSelector) : base(new ShortDomainProvider<TSource>(valueSelector)) {}
    }
}