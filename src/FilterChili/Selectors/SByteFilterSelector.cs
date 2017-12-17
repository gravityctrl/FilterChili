using System;
using System.Linq.Expressions;
using GravityCTRL.FilterChili.Providers;

namespace GravityCTRL.FilterChili.Selectors
{
    public class SByteFilterSelector<TSource> : FilterSelector<TSource, sbyte, SByteDomainProvider<TSource>>
    {
        internal SByteFilterSelector(Expression<Func<TSource, sbyte>> valueSelector) : base(new SByteDomainProvider<TSource>(valueSelector)) {}
    }
}