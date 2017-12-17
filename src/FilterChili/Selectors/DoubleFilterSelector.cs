using System;
using System.Linq.Expressions;
using GravityCTRL.FilterChili.Providers;

namespace GravityCTRL.FilterChili.Selectors
{
    public class DoubleFilterSelector<TSource> : FilterSelector<TSource, double, DoubleDomainProvider<TSource>>
    {
        internal DoubleFilterSelector(Expression<Func<TSource, double>> valueSelector) : base(new DoubleDomainProvider<TSource>(valueSelector)) {}
    }
}