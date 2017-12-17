using System;
using System.Linq.Expressions;
using GravityCTRL.FilterChili.Providers;

namespace GravityCTRL.FilterChili.Selectors
{
    public class ByteFilterSelector<TSource> : FilterSelector<TSource, byte, ByteDomainProvider<TSource>>
    {
        internal ByteFilterSelector(Expression<Func<TSource, byte>> valueSelector) : base(new ByteDomainProvider<TSource>(valueSelector)) {}
    }
}