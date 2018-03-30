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
    public sealed class LongFilterSelector<TSource> : FilterSelector<TSource, long>
    {
        internal LongFilterSelector(Expression<Func<TSource, long>> selector) : base(selector) {}

        [NotNull]
        [UsedImplicitly]
        public RangeResolver<TSource, long> WithRange()
        {
            var resolver = new RangeResolver<TSource, long>(Selector, long.MinValue, long.MaxValue);
            DomainResolver = resolver;
            return resolver;
        }

        [NotNull]
        [UsedImplicitly]
        public ComparisonResolver<TSource, long> WithGreaterThan()
        {
            var resolver = new ComparisonResolver<TSource, long>(new GreaterThanComparer<TSource, long>(long.MinValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [NotNull]
        [UsedImplicitly]
        public ComparisonResolver<TSource, long> WithLessThan()
        {
            var resolver = new ComparisonResolver<TSource, long>(new LessThanComparer<TSource, long>(long.MaxValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [NotNull]
        [UsedImplicitly]
        public ComparisonResolver<TSource, long> WithGreaterThanOrEqual()
        {
            var resolver = new ComparisonResolver<TSource, long>(new GreaterThanOrEqualComparer<TSource, long>(long.MinValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [NotNull]
        [UsedImplicitly]
        public ComparisonResolver<TSource, long> WithLessThanOrEqual()
        {
            var resolver = new ComparisonResolver<TSource, long>(new LessThanOrEqualComparer<TSource, long>(long.MaxValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }
    }
}