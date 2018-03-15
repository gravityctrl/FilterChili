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
    public class LongFilterSelector<TSource> : FilterSelector<TSource, long>
    {
        internal LongFilterSelector(Expression<Func<TSource, long>> selector) : base(selector) {}

        [UsedImplicitly]
        public LongRangeResolver<TSource> Range(string name)
        {
            var resolver = new LongRangeResolver<TSource>(name, Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public LongComparisonResolver<TSource> GreaterThan(string name)
        {
            var resolver = new LongComparisonResolver<TSource>(name, new GreaterThanComparer<TSource, long>(long.MinValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public LongComparisonResolver<TSource> LessThan(string name)
        {
            var resolver = new LongComparisonResolver<TSource>(name, new LessThanComparer<TSource, long>(long.MaxValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public LongComparisonResolver<TSource> GreaterThanOrEqual(string name)
        {
            var resolver = new LongComparisonResolver<TSource>(name, new GreaterThanOrEqualComparer<TSource, long>(long.MinValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public LongComparisonResolver<TSource> LessThanOrEqual(string name)
        {
            var resolver = new LongComparisonResolver<TSource>(name, new LessThanOrEqualComparer<TSource, long>(long.MaxValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }
    }
}