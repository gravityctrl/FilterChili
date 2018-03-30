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
    public sealed class UIntFilterSelector<TSource> : FilterSelector<TSource, uint>
    {
        internal UIntFilterSelector(Expression<Func<TSource, uint>> selector) : base(selector) {}

        [NotNull]
        [UsedImplicitly]
        public RangeResolver<TSource, uint> WithRange()
        {
            var resolver = new RangeResolver<TSource, uint>(Selector, uint.MinValue, uint.MaxValue);
            DomainResolver = resolver;
            return resolver;
        }

        [NotNull]
        [UsedImplicitly]
        public ComparisonResolver<TSource, uint> WithGreaterThan()
        {
            var resolver = new ComparisonResolver<TSource, uint>(new GreaterThanComparer<TSource, uint>(uint.MinValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [NotNull]
        [UsedImplicitly]
        public ComparisonResolver<TSource, uint> WithLessThan()
        {
            var resolver = new ComparisonResolver<TSource, uint>(new LessThanComparer<TSource, uint>(uint.MaxValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [NotNull]
        [UsedImplicitly]
        public ComparisonResolver<TSource, uint> WithGreaterThanOrEqual()
        {
            var resolver = new ComparisonResolver<TSource, uint>(new GreaterThanOrEqualComparer<TSource, uint>(uint.MinValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [NotNull]
        [UsedImplicitly]
        public ComparisonResolver<TSource, uint> WithLessThanOrEqual()
        {
            var resolver = new ComparisonResolver<TSource, uint>(new LessThanOrEqualComparer<TSource, uint>(uint.MaxValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }
    }
}