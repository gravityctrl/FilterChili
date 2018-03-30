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
    public sealed class FloatFilterSelector<TSource> : FilterSelector<TSource, float>
    {
        internal FloatFilterSelector(Expression<Func<TSource, float>> selector) : base(selector) {}

        [NotNull]
        [UsedImplicitly]
        public RangeResolver<TSource, float> WithRange()
        {
            var resolver = new RangeResolver<TSource, float>(Selector, float.MinValue, float.MaxValue);
            DomainResolver = resolver;
            return resolver;
        }

        [NotNull]
        [UsedImplicitly]
        public ComparisonResolver<TSource, float> WithGreaterThan()
        {
            var resolver = new ComparisonResolver<TSource, float>(new GreaterThanComparer<TSource, float>(float.MinValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [NotNull]
        [UsedImplicitly]
        public ComparisonResolver<TSource, float> WithLessThan()
        {
            var resolver = new ComparisonResolver<TSource, float>(new LessThanComparer<TSource, float>(float.MaxValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [NotNull]
        [UsedImplicitly]
        public ComparisonResolver<TSource, float> WithGreaterThanOrEqual()
        {
            var resolver = new ComparisonResolver<TSource, float>(new GreaterThanOrEqualComparer<TSource, float>(float.MinValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [NotNull]
        [UsedImplicitly]
        public ComparisonResolver<TSource, float> WithLessThanOrEqual()
        {
            var resolver = new ComparisonResolver<TSource, float>(new LessThanOrEqualComparer<TSource, float>(float.MaxValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }
    }
}