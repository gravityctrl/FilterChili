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

namespace GravityCTRL.FilterChili.Providers
{
    public class ByteDomainProvider<TSource> : DomainProvider<TSource, byte>
    {
        internal ByteDomainProvider(Expression<Func<TSource, byte>> selector) : base(selector) {}

        [UsedImplicitly]
        public ByteRangeResolver<TSource> Range(string name)
        {
            return new ByteRangeResolver<TSource>(name, Selector);
        }

        [UsedImplicitly]
        public ByteComparisonResolver<TSource> GreaterThan(string name)
        {
            return new ByteComparisonResolver<TSource>(name, new GreaterThanComparer<TSource, byte>(byte.MinValue), Selector);
        }

        [UsedImplicitly]
        public ByteComparisonResolver<TSource> LessThan(string name)
        {
            return new ByteComparisonResolver<TSource>(name, new LessThanComparer<TSource, byte>(byte.MaxValue), Selector);
        }

        [UsedImplicitly]
        public ByteComparisonResolver<TSource> GreaterThanOrEqual(string name)
        {
            return new ByteComparisonResolver<TSource>(name, new GreaterThanOrEqualComparer<TSource, byte>(byte.MinValue), Selector);
        }

        [UsedImplicitly]
        public ByteComparisonResolver<TSource> LessThanOrEqual(string name)
        {
            return new ByteComparisonResolver<TSource>(name, new LessThanOrEqualComparer<TSource, byte>(byte.MaxValue), Selector);
        }
    }
}