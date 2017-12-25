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
    public class ULongDomainProvider<TSource> : DomainProvider<TSource, ulong>
    {
        internal ULongDomainProvider(Expression<Func<TSource, ulong>> selector) : base(selector) {}

        [UsedImplicitly]
        public ULongRangeResolver<TSource> Range(string name)
        {
            return new ULongRangeResolver<TSource>(name, Selector);
        }

        [UsedImplicitly]
        public ULongComparisonResolver<TSource> GreaterThan(string name)
        {
            return new ULongComparisonResolver<TSource>(name, new GreaterThanComparer<TSource, ulong>(ulong.MinValue), Selector);
        }

        [UsedImplicitly]
        public ULongComparisonResolver<TSource> LessThan(string name)
        {
            return new ULongComparisonResolver<TSource>(name, new LessThanComparer<TSource, ulong>(ulong.MaxValue), Selector);
        }

        [UsedImplicitly]
        public ULongComparisonResolver<TSource> GreaterThanOrEqual(string name)
        {
            return new ULongComparisonResolver<TSource>(name, new GreaterThanOrEqualComparer<TSource, ulong>(ulong.MinValue), Selector);
        }

        [UsedImplicitly]
        public ULongComparisonResolver<TSource> LessThanOrEqual(string name)
        {
            return new ULongComparisonResolver<TSource>(name, new LessThanOrEqualComparer<TSource, ulong>(ulong.MaxValue), Selector);
        }
    }
}