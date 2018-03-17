using System;
using System.Linq.Expressions;
using GravityCTRL.FilterChili.Comparison;

namespace GravityCTRL.FilterChili.Resolvers.Comparison
{
    public class FloatComparisonResolver<TSource> : ComparisonResolver<TSource, float>
    {
        internal FloatComparisonResolver(Comparer<TSource, float> comparer, Expression<Func<TSource, float>> selector) : base(comparer, selector) { }
    }
}