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
using GravityCTRL.FilterChili.Resolvers.Comparison;
using GravityCTRL.FilterChili.Resolvers.Range;
using JetBrains.Annotations;

namespace GravityCTRL.FilterChili.Selectors
{
    public class SByteFilterSelector<TSource> : FilterSelector<TSource, sbyte>
    {
        internal SByteFilterSelector(Expression<Func<TSource, sbyte>> selector) : base(selector) {}

        [UsedImplicitly]
        public SByteRangeResolver<TSource> Range(string name)
        {
            var resolver = new SByteRangeResolver<TSource>(name, Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public SByteComparisonResolver<TSource> GreaterThan(string name)
        {
            var resolver = new SByteComparisonResolver<TSource>(name, new GreaterThanComparer<TSource, sbyte>(sbyte.MinValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public SByteComparisonResolver<TSource> LessThan(string name)
        {
            var resolver = new SByteComparisonResolver<TSource>(name, new LessThanComparer<TSource, sbyte>(sbyte.MaxValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public SByteComparisonResolver<TSource> GreaterThanOrEqual(string name)
        {
            var resolver = new SByteComparisonResolver<TSource>(name, new GreaterThanOrEqualComparer<TSource, sbyte>(sbyte.MinValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public SByteComparisonResolver<TSource> LessThanOrEqual(string name)
        {
            var resolver = new SByteComparisonResolver<TSource>(name, new LessThanOrEqualComparer<TSource, sbyte>(sbyte.MaxValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }
    }
}