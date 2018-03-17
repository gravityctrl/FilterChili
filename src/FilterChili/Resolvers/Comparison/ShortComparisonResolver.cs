using System;
using System.Linq.Expressions;
using GravityCTRL.FilterChili.Comparison;

namespace GravityCTRL.FilterChili.Resolvers.Comparison
{
    public class ShortComparisonResolver<TSource> : ComparisonResolver<TSource, short>
    {
        internal ShortComparisonResolver(Comparer<TSource, short> comparer, Expression<Func<TSource, short>> selector) : base(comparer, selector) { }
    }
}