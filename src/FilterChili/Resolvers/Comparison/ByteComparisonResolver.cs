using System;
using System.Linq.Expressions;
using GravityCTRL.FilterChili.Comparison;

namespace GravityCTRL.FilterChili.Resolvers.Comparison
{
    public class ByteComparisonResolver<TSource> : ComparisonResolver<TSource, byte>
    {
        internal ByteComparisonResolver(Comparer<TSource, byte> comparer, Expression<Func<TSource, byte>> selector) : base(comparer, selector) { }
    }
}