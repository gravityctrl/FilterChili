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
    public sealed class ShortFilterSelector<TSource> : FilterSelector<TSource, short>
    {
        internal ShortFilterSelector(Expression<Func<TSource, short>> selector) : base(selector) {}

        [NotNull]
        [UsedImplicitly]
        public RangeResolver<TSource, short> WithRange()
        {
            var resolver = new RangeResolver<TSource, short>(Selector, short.MinValue, short.MaxValue);
            FilterResolver = resolver;
            return resolver;
        }

        [NotNull]
        [UsedImplicitly]
        public ComparisonResolver<TSource, short> WithGreaterThan()
        {
            var resolver = new ComparisonResolver<TSource, short>(new GreaterThanComparer<TSource, short>(short.MinValue), Selector);
            FilterResolver = resolver;
            return resolver;
        }

        [NotNull]
        [UsedImplicitly]
        public ComparisonResolver<TSource, short> WithLessThan()
        {
            var resolver = new ComparisonResolver<TSource, short>(new LessThanComparer<TSource, short>(short.MaxValue), Selector);
            FilterResolver = resolver;
            return resolver;
        }

        [NotNull]
        [UsedImplicitly]
        public ComparisonResolver<TSource, short> WithGreaterThanOrEqual()
        {
            var resolver = new ComparisonResolver<TSource, short>(new GreaterThanOrEqualComparer<TSource, short>(short.MinValue), Selector);
            FilterResolver = resolver;
            return resolver;
        }

        [NotNull]
        [UsedImplicitly]
        public ComparisonResolver<TSource, short> WithLessThanOrEqual()
        {
            var resolver = new ComparisonResolver<TSource, short>(new LessThanOrEqualComparer<TSource, short>(short.MaxValue), Selector);
            FilterResolver = resolver;
            return resolver;
        }
    }
}