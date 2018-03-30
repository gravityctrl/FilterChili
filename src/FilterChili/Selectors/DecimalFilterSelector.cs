﻿// This file is part of FilterChili.
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
    public sealed class DecimalFilterSelector<TSource> : FilterSelector<TSource, decimal>
    {
        internal DecimalFilterSelector(Expression<Func<TSource, decimal>> selector) : base(selector) {}

        [NotNull]
        [UsedImplicitly]
        public RangeResolver<TSource, decimal> WithRange()
        {
            var resolver = new RangeResolver<TSource, decimal>(Selector, decimal.MinValue, decimal.MaxValue);
            DomainResolver = resolver;
            return resolver;
        }

        [NotNull]
        [UsedImplicitly]
        public ComparisonResolver<TSource, decimal> WithGreaterThan()
        {
            var resolver = new ComparisonResolver<TSource, decimal>(new GreaterThanComparer<TSource, decimal>(decimal.MinValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [NotNull]
        [UsedImplicitly]
        public ComparisonResolver<TSource, decimal> WithLessThan()
        {
            var resolver = new ComparisonResolver<TSource, decimal>(new LessThanComparer<TSource, decimal>(decimal.MaxValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [NotNull]
        [UsedImplicitly]
        public ComparisonResolver<TSource, decimal> WithGreaterThanOrEqual()
        {
            var resolver = new ComparisonResolver<TSource, decimal>(new GreaterThanOrEqualComparer<TSource, decimal>(decimal.MinValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [NotNull]
        [UsedImplicitly]
        public ComparisonResolver<TSource, decimal> WithLessThanOrEqual()
        {
            var resolver = new ComparisonResolver<TSource, decimal>(new LessThanOrEqualComparer<TSource, decimal>(decimal.MaxValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }
    }
}