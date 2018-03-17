using System;
using System.Linq.Expressions;
using GravityCTRL.FilterChili.Comparison;

namespace GravityCTRL.FilterChili.Resolvers.Comparison
{
    public class SByteComparisonResolver<TSource> : ComparisonResolver<TSource, sbyte>
    {
        internal SByteComparisonResolver(Comparer<TSource, sbyte> comparer, Expression<Func<TSource, sbyte>> selector) : base(comparer, selector) { }
    }
}