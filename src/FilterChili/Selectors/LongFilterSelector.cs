using System;
using System.Linq.Expressions;
using GravityCTRL.FilterChili.Providers;

namespace GravityCTRL.FilterChili.Selectors
{
    public class LongFilterSelector<TSource> : FilterSelector<TSource, long, LongDomainProvider<TSource>>
    {
        internal LongFilterSelector(Expression<Func<TSource, long>> valueSelector) : base(new LongDomainProvider<TSource>(valueSelector)) {}
    }
}