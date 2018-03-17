using System;
using System.Linq.Expressions;

namespace GravityCTRL.FilterChili.Resolvers.Range
{
    public class ByteRangeResolver<TSource> : RangeResolver<TSource, byte>
    {
        internal ByteRangeResolver(Expression<Func<TSource, byte>> selector) : base(selector, byte.MinValue, byte.MaxValue) {}
    }
}