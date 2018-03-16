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
    public class UShortFilterSelector<TSource> : FilterSelector<TSource, ushort>
    {
        internal UShortFilterSelector(Expression<Func<TSource, ushort>> selector) : base(selector) {}

        [UsedImplicitly]
        public UShortRangeResolver<TSource> WithRange()
        {
            var resolver = new UShortRangeResolver<TSource>(Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public UShortComparisonResolver<TSource> WithGreaterThan()
        {
            var resolver = new UShortComparisonResolver<TSource>(new GreaterThanComparer<TSource, ushort>(ushort.MinValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public UShortComparisonResolver<TSource> WithLessThan()
        {
            var resolver = new UShortComparisonResolver<TSource>(new LessThanComparer<TSource, ushort>(ushort.MaxValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public UShortComparisonResolver<TSource> WithGreaterThanOrEqual()
        {
            var resolver = new UShortComparisonResolver<TSource>(new GreaterThanOrEqualComparer<TSource, ushort>(ushort.MinValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public UShortComparisonResolver<TSource> WithLessThanOrEqual()
        {
            var resolver = new UShortComparisonResolver<TSource>(new LessThanOrEqualComparer<TSource, ushort>(ushort.MaxValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }
    }
}