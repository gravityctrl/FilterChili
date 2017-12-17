using System;
using System.Linq.Expressions;
using GravityCTRL.FilterChili.Providers;

namespace GravityCTRL.FilterChili.Selectors
{
    public class FloatFilterSelector<TSource> : FilterSelector<TSource, float, FloatDomainProvider<TSource>>
    {
        internal FloatFilterSelector(Expression<Func<TSource, float>> valueSelector) : base(new FloatDomainProvider<TSource>(valueSelector)) {}
    }
}