using System;
using System.Linq.Expressions;

namespace GravityCTRL.FilterChili.Resolvers.Range
{
    public class IntRangeResolver<TSource> : RangeResolver<TSource, int>
    {
        internal IntRangeResolver(Expression<Func<TSource, int>> selector) : base(selector, int.MinValue, int.MaxValue) { }
    }
}