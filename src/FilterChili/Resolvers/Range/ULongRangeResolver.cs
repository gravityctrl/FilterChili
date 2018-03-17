using System;
using System.Linq.Expressions;

namespace GravityCTRL.FilterChili.Resolvers.Range
{
    public class ULongRangeResolver<TSource> : RangeResolver<TSource, ulong>
    {
        internal ULongRangeResolver(Expression<Func<TSource, ulong>> selector) : base(selector, ulong.MinValue, ulong.MaxValue) { }
    }
}