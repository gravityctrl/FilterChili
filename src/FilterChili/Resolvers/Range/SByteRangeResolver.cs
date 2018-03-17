using System;
using System.Linq.Expressions;

namespace GravityCTRL.FilterChili.Resolvers.Range
{
    public class SByteRangeResolver<TSource> : RangeResolver<TSource, sbyte>
    {
        internal SByteRangeResolver(Expression<Func<TSource, sbyte>> selector) : base(selector, sbyte.MinValue, sbyte.MaxValue) { }
    }
}