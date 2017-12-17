using System;
using System.Linq.Expressions;
using GravityCTRL.FilterChili.Providers;

namespace GravityCTRL.FilterChili.Selectors
{
    public class ULongFilterSelector<TSource> : FilterSelector<TSource, ulong, ULongDomainProvider<TSource>>
    {
        internal ULongFilterSelector(Expression<Func<TSource, ulong>> valueSelector) : base(new ULongDomainProvider<TSource>(valueSelector)) {}
    }
}