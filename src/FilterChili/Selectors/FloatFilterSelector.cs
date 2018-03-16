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
    public class FloatFilterSelector<TSource> : FilterSelector<TSource, float>
    {
        internal FloatFilterSelector(Expression<Func<TSource, float>> selector) : base(selector) {}

        [UsedImplicitly]
        public FloatRangeResolver<TSource> Range()
        {
            var resolver = new FloatRangeResolver<TSource>(Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public FloatComparisonResolver<TSource> GreaterThan()
        {
            var resolver = new FloatComparisonResolver<TSource>(new GreaterThanComparer<TSource, float>(float.MinValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public FloatComparisonResolver<TSource> LessThan()
        {
            var resolver = new FloatComparisonResolver<TSource>(new LessThanComparer<TSource, float>(float.MaxValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public FloatComparisonResolver<TSource> GreaterThanOrEqual()
        {
            var resolver = new FloatComparisonResolver<TSource>(new GreaterThanOrEqualComparer<TSource, float>(float.MinValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }

        [UsedImplicitly]
        public FloatComparisonResolver<TSource> LessThanOrEqual()
        {
            var resolver = new FloatComparisonResolver<TSource>(new LessThanOrEqualComparer<TSource, float>(float.MaxValue), Selector);
            DomainResolver = resolver;
            return resolver;
        }
    }
}