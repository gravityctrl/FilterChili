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
    public class SByteFilterSelector<TSource> : FilterSelector<TSource, sbyte>
    {
        internal SByteFilterSelector(Expression<Func<TSource, sbyte>> selector) : base(selector) {}

        [UsedImplicitly]
        public RangeResolver<TSource, sbyte> WithRange()
        {
            var resolver = new SByteRangeResolver<TSource>(Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public ComparisonResolver<TSource, sbyte> WithGreaterThan()
        {
            var resolver = new SByteComparisonResolver<TSource>(new GreaterThanComparer<TSource, sbyte>(sbyte.MinValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public ComparisonResolver<TSource, sbyte> WithLessThan()
        {
            var resolver = new SByteComparisonResolver<TSource>(new LessThanComparer<TSource, sbyte>(sbyte.MaxValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public ComparisonResolver<TSource, sbyte> WithGreaterThanOrEqual()
        {
            var resolver = new SByteComparisonResolver<TSource>(new GreaterThanOrEqualComparer<TSource, sbyte>(sbyte.MinValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public ComparisonResolver<TSource, sbyte> WithLessThanOrEqual()
        {
            var resolver = new SByteComparisonResolver<TSource>(new LessThanOrEqualComparer<TSource, sbyte>(sbyte.MaxValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }
    }
}