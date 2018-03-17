using System;
using System.Linq.Expressions;

namespace GravityCTRL.FilterChili.Resolvers.Range
{
    public class DoubleRangeResolver<TSource> : RangeResolver<TSource, double>
    {
        internal DoubleRangeResolver(Expression<Func<TSource, double>> selector) : base(selector, double.MinValue, double.MaxValue) { }
    }
}