using System;
using System.Linq.Expressions;
using GravityCTRL.FilterChili.Comparison;

namespace GravityCTRL.FilterChili.Resolvers.Comparison
{
    public class CharComparisonResolver<TSource> : ComparisonResolver<TSource, char>
    {
        internal CharComparisonResolver(Comparer<TSource, char> comparer, Expression<Func<TSource, char>> selector) : base(comparer, selector) { }
    }
}