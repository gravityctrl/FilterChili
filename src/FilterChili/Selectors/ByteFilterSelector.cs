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
    public class ByteFilterSelector<TSource> : FilterSelector<TSource, byte>
    {
        internal ByteFilterSelector(Expression<Func<TSource, byte>> selector) : base(selector) {}

        [UsedImplicitly]
        public RangeResolver<TSource, byte> WithRange()
        {
            var resolver = new RangeResolver<TSource, byte>(Selector, byte.MinValue, byte.MaxValue);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public ComparisonResolver<TSource, byte> WithGreaterThan()
        {
            var resolver = new ComparisonResolver<TSource, byte>(new GreaterThanComparer<TSource, byte>(byte.MinValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public ComparisonResolver<TSource, byte> WithLessThan()
        {
            var resolver = new ComparisonResolver<TSource, byte>(new LessThanComparer<TSource, byte>(byte.MaxValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public ComparisonResolver<TSource, byte> WithGreaterThanOrEqual()
        {
            var resolver = new ComparisonResolver<TSource, byte>(new GreaterThanOrEqualComparer<TSource, byte>(byte.MinValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public ComparisonResolver<TSource, byte> WithLessThanOrEqual()
        {
            var resolver = new ComparisonResolver<TSource, byte>(new LessThanOrEqualComparer<TSource, byte>(byte.MaxValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }
    }
}