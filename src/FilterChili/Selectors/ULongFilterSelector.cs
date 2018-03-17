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
using GravityCTRL.FilterChili.Resolvers;
using JetBrains.Annotations;

namespace GravityCTRL.FilterChili.Selectors
{
    public class ULongFilterSelector<TSource> : FilterSelector<TSource, ulong>
    {
        internal ULongFilterSelector(Expression<Func<TSource, ulong>> selector) : base(selector) {}

        [UsedImplicitly]
        public RangeResolver<TSource, ulong> WithRange()
        {
            var resolver = new ULongRangeResolver<TSource>(Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public ComparisonResolver<TSource, ulong> WithGreaterThan()
        {
            var resolver = new ULongComparisonResolver<TSource>(new GreaterThanComparer<TSource, ulong>(ulong.MinValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public ComparisonResolver<TSource, ulong> WithLessThan()
        {
            var resolver = new ULongComparisonResolver<TSource>(new LessThanComparer<TSource, ulong>(ulong.MaxValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public ComparisonResolver<TSource, ulong> WithGreaterThanOrEqual()
        {
            var resolver = new ULongComparisonResolver<TSource>(new GreaterThanOrEqualComparer<TSource, ulong>(ulong.MinValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public ComparisonResolver<TSource, ulong> WithLessThanOrEqual()
        {
            var resolver = new ULongComparisonResolver<TSource>(new LessThanOrEqualComparer<TSource, ulong>(ulong.MaxValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }
    }
}