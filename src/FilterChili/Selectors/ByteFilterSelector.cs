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
        public ByteRangeResolver<TSource> WithRange()
        {
            var resolver = new ByteRangeResolver<TSource>(Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public ByteComparisonResolver<TSource> WithGreaterThan()
        {
            var resolver = new ByteComparisonResolver<TSource>(new GreaterThanComparer<TSource, byte>(byte.MinValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public ByteComparisonResolver<TSource> WithLessThan()
        {
            var resolver = new ByteComparisonResolver<TSource>(new LessThanComparer<TSource, byte>(byte.MaxValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public ByteComparisonResolver<TSource> WithGreaterThanOrEqual()
        {
            var resolver = new ByteComparisonResolver<TSource>(new GreaterThanOrEqualComparer<TSource, byte>(byte.MinValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public ByteComparisonResolver<TSource> WithLessThanOrEqual()
        {
            var resolver = new ByteComparisonResolver<TSource>(new LessThanOrEqualComparer<TSource, byte>(byte.MaxValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }
    }
}