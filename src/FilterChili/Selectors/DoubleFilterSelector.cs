// This file is part of FilterChili.
// Copyright © 2017 Sebastian Krogull.
// 
// FilterChili is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as 
// published by the Free Software Foundation, either version 3 
// of the License, or any later version.
// 
// FilterChili is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public 
// License along with FilterChili. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Linq.Expressions;
using GravityCTRL.FilterChili.Comparison;
using JetBrains.Annotations;

namespace GravityCTRL.FilterChili.Selectors
{
    public class DoubleFilterSelector<TSource> : FilterSelector<TSource, double>
    {
        internal DoubleFilterSelector(Expression<Func<TSource, double>> selector) : base(selector) {}

        [UsedImplicitly]
        public RangeResolver<TSource, double> WithRange()
        {
            var resolver = new RangeResolver<TSource, double>(Selector, double.MinValue, double.MaxValue);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public ComparisonResolver<TSource, double> WithGreaterThan()
        {
            var resolver = new ComparisonResolver<TSource, double>(new GreaterThanComparer<TSource, double>(double.MinValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public ComparisonResolver<TSource, double> WithLessThan()
        {
            var resolver = new ComparisonResolver<TSource, double>(new LessThanComparer<TSource, double>(double.MaxValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public ComparisonResolver<TSource, double> WithGreaterThanOrEqual()
        {
            var resolver = new ComparisonResolver<TSource, double>(new GreaterThanOrEqualComparer<TSource, double>(double.MinValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public ComparisonResolver<TSource, double> WithLessThanOrEqual()
        {
            var resolver = new ComparisonResolver<TSource, double>(new LessThanOrEqualComparer<TSource, double>(double.MaxValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }
    }
}