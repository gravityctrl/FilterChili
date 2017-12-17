using System;
using System.Linq.Expressions;
using GravityCTRL.FilterChili.Providers;

namespace GravityCTRL.FilterChili.Selectors
{
    public class UIntFilterSelector<TSource> : FilterSelector<TSource, uint, UIntDomainProvider<TSource>>
    {
        internal UIntFilterSelector(Expression<Func<TSource, uint>> valueSelector) : base(new UIntDomainProvider<TSource>(valueSelector)) {}
    }
}