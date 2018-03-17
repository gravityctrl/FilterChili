using System;
using System.Linq.Expressions;

namespace GravityCTRL.FilterChili.Resolvers.Range
{
    public class DecimalRangeResolver<TSource> : RangeResolver<TSource, decimal>
    {
        internal DecimalRangeResolver(Expression<Func<TSource, decimal>> selector) : base(selector, decimal.MinValue, decimal.MaxValue) { }
    }
}