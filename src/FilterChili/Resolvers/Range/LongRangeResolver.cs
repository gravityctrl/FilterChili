using System;
using System.Linq.Expressions;

namespace GravityCTRL.FilterChili.Resolvers.Range
{
    public class LongRangeResolver<TSource> : RangeResolver<TSource, long>
    {
        internal LongRangeResolver(Expression<Func<TSource, long>> selector) : base(selector, long.MinValue, long.MaxValue) { }
    }
}