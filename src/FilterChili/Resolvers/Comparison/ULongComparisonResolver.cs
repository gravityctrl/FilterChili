using System;
using System.Linq.Expressions;
using GravityCTRL.FilterChili.Comparison;

namespace GravityCTRL.FilterChili.Resolvers.Comparison
{
    public class ULongComparisonResolver<TSource> : ComparisonResolver<TSource, ulong>
    {
        internal ULongComparisonResolver(Comparer<TSource, ulong> comparer, Expression<Func<TSource, ulong>> selector) : base(comparer, selector) { }
    }
}