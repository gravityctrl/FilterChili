using System;
using System.Linq.Expressions;

namespace GravityCTRL.FilterChili.Resolvers.Range
{
    public class CharRangeResolver<TSource> : RangeResolver<TSource, char>
    {
        internal CharRangeResolver(Expression<Func<TSource, char>> selector) : base(selector, char.MinValue, char.MaxValue) { }
    }
}