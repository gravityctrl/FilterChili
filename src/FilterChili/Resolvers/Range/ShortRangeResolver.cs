using System;
using System.Linq.Expressions;

namespace GravityCTRL.FilterChili.Resolvers.Range
{
    public class ShortRangeResolver<TSource> : RangeResolver<TSource, short>
    {
        internal ShortRangeResolver(Expression<Func<TSource, short>> selector) : base(selector, short.MinValue, short.MaxValue) { }
    }
}