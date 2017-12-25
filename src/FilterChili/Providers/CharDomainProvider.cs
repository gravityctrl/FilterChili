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
    public class CharDomainProvider<TSource> : DomainProvider<TSource, char>
    {
        internal CharDomainProvider(Expression<Func<TSource, char>> selector) : base(selector) {}

        [UsedImplicitly]
        public CharRangeResolver<TSource> Range(string name)
        {
            return new CharRangeResolver<TSource>(name, Selector);
        }

        [UsedImplicitly]
        public CharComparisonResolver<TSource> GreaterThan(string name)
        {
            return new CharComparisonResolver<TSource>(name, new GreaterThanComparer<TSource, char>(char.MinValue), Selector);
        }

        [UsedImplicitly]
        public CharComparisonResolver<TSource> LessThan(string name)
        {
            return new CharComparisonResolver<TSource>(name, new LessThanComparer<TSource, char>(char.MaxValue), Selector);
        }

        [UsedImplicitly]
        public CharComparisonResolver<TSource> GreaterThanOrEqual(string name)
        {
            return new CharComparisonResolver<TSource>(name, new GreaterThanOrEqualComparer<TSource, char>(char.MinValue), Selector);
        }

        [UsedImplicitly]
        public CharComparisonResolver<TSource> LessThanOrEqual(string name)
        {
            return new CharComparisonResolver<TSource>(name, new LessThanOrEqualComparer<TSource, char>(char.MaxValue), Selector);
        }
    }
}