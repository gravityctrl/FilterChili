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
    public class UShortFilterSelector<TSource> : FilterSelector<TSource, ushort>
    {
        internal UShortFilterSelector(Expression<Func<TSource, ushort>> selector) : base(selector) {}

        [UsedImplicitly]
        public RangeResolver<TSource, ushort> WithRange()
        {
            var resolver = new RangeResolver<TSource, ushort>(Selector, ushort.MinValue, ushort.MaxValue);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public ComparisonResolver<TSource, ushort> WithGreaterThan()
        {
            var resolver = new ComparisonResolver<TSource, ushort>(new GreaterThanComparer<TSource, ushort>(ushort.MinValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public ComparisonResolver<TSource, ushort> WithLessThan()
        {
            var resolver = new ComparisonResolver<TSource, ushort>(new LessThanComparer<TSource, ushort>(ushort.MaxValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public ComparisonResolver<TSource, ushort> WithGreaterThanOrEqual()
        {
            var resolver = new ComparisonResolver<TSource, ushort>(new GreaterThanOrEqualComparer<TSource, ushort>(ushort.MinValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public ComparisonResolver<TSource, ushort> WithLessThanOrEqual()
        {
            var resolver = new ComparisonResolver<TSource, ushort>(new LessThanOrEqualComparer<TSource, ushort>(ushort.MaxValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }
    }
}