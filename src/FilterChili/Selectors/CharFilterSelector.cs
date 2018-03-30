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
    public sealed class CharFilterSelector<TSource> : FilterSelector<TSource, char>
    {
        internal CharFilterSelector(Expression<Func<TSource, char>> selector) : base(selector) {}

        [NotNull]
        [UsedImplicitly]
        public RangeResolver<TSource, char> WithRange()
        {
            var resolver = new RangeResolver<TSource, char>(Selector, char.MinValue, char.MaxValue);
            DomainResolver = resolver;
            return resolver;
        }

        [NotNull]
        [UsedImplicitly]
        public ComparisonResolver<TSource, char> WithGreaterThan()
        {
            var resolver = new ComparisonResolver<TSource, char>(new GreaterThanComparer<TSource, char>(char.MinValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [NotNull]
        [UsedImplicitly]
        public ComparisonResolver<TSource, char> WithLessThan()
        {
            var resolver = new ComparisonResolver<TSource, char>(new LessThanComparer<TSource, char>(char.MaxValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [NotNull]
        [UsedImplicitly]
        public ComparisonResolver<TSource, char> WithGreaterThanOrEqual()
        {
            var resolver = new ComparisonResolver<TSource, char>(new GreaterThanOrEqualComparer<TSource, char>(char.MinValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [NotNull]
        [UsedImplicitly]
        public ComparisonResolver<TSource, char> WithLessThanOrEqual()
        {
            var resolver = new ComparisonResolver<TSource, char>(new LessThanOrEqualComparer<TSource, char>(char.MaxValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }
    }
}