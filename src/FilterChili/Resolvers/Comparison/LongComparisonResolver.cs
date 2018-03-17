using System;
using System.Linq.Expressions;
using GravityCTRL.FilterChili.Comparison;

namespace GravityCTRL.FilterChili.Resolvers.Comparison
{
    public class LongComparisonResolver<TSource> : ComparisonResolver<TSource, long>
    {
        internal LongComparisonResolver(Comparer<TSource, long> comparer, Expression<Func<TSource, long>> selector) : base(comparer, selector) { }
    }
}