using System;
using System.Linq.Expressions;
using GravityCTRL.FilterChili.Comparison;

namespace GravityCTRL.FilterChili.Resolvers.Comparison
{
    public class IntComparisonResolver<TSource> : ComparisonResolver<TSource, int>
    {
        internal IntComparisonResolver(Comparer<TSource, int> comparer, Expression<Func<TSource, int>> selector) : base(comparer, selector) { }
    }
}