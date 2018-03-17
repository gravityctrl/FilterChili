using System;
using System.Linq.Expressions;
using GravityCTRL.FilterChili.Comparison;

namespace GravityCTRL.FilterChili.Resolvers.Comparison
{
    public class UIntComparisonResolver<TSource> : ComparisonResolver<TSource, uint>
    {
        internal UIntComparisonResolver(Comparer<TSource, uint> comparer, Expression<Func<TSource, uint>> selector) : base(comparer, selector) { }
    }
}