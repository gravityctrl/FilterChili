using System;
using System.Linq.Expressions;
using GravityCTRL.FilterChili.Comparison;

namespace GravityCTRL.FilterChili.Resolvers.Comparison
{
    public class DoubleComparisonResolver<TSource> : ComparisonResolver<TSource, double>
    {
        internal DoubleComparisonResolver(Comparer<TSource, double> comparer, Expression<Func<TSource, double>> selector) : base(comparer, selector) { }
    }
}