using System;
using System.Linq.Expressions;

namespace GravityCTRL.FilterChili.Resolvers.Range
{
    public class UIntRangeResolver<TSource> : RangeResolver<TSource, uint>
    {
        internal UIntRangeResolver(Expression<Func<TSource, uint>> selector) : base(selector, uint.MinValue, uint.MaxValue) { }
    }
}