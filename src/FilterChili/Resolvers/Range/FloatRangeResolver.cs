using System;
using System.Linq.Expressions;

namespace GravityCTRL.FilterChili.Resolvers.Range
{
    public class FloatRangeResolver<TSource> : RangeResolver<TSource, float>
    {
        internal FloatRangeResolver(Expression<Func<TSource, float>> selector) : base(selector, float.MinValue, float.MaxValue) { }
    }
}