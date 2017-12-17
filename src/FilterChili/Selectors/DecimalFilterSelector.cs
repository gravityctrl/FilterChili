using System;
using System.Linq.Expressions;
using GravityCTRL.FilterChili.Providers;

namespace GravityCTRL.FilterChili.Selectors
{
    public class DecimalFilterSelector<TSource> : FilterSelector<TSource, decimal, DecimalDomainProvider<TSource>>
    {
        internal DecimalFilterSelector(Expression<Func<TSource, decimal>> valueSelector) : base(new DecimalDomainProvider<TSource>(valueSelector)) {}
    }
}