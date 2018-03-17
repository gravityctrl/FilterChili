using System;
using System.Linq.Expressions;
using GravityCTRL.FilterChili.Comparison;

namespace GravityCTRL.FilterChili.Resolvers.Comparison
{
    public class DecimalComparisonResolver<TSource> : ComparisonResolver<TSource, decimal>
    {
        internal DecimalComparisonResolver(Comparer<TSource, decimal> comparer, Expression<Func<TSource, decimal>> selector) : base(comparer, selector) { }
    }
}