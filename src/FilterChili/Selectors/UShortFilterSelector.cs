using System;
using System.Linq.Expressions;
using GravityCTRL.FilterChili.Providers;

namespace GravityCTRL.FilterChili.Selectors
{
    public class UShortFilterSelector<TSource> : FilterSelector<TSource, ushort, UShortDomainProvider<TSource>>
    {
        internal UShortFilterSelector(Expression<Func<TSource, ushort>> valueSelector) : base(new UShortDomainProvider<TSource>(valueSelector)) {}
    }
}