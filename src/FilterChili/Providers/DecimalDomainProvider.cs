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
using GravityCTRL.FilterChili.Resolvers.Comparison;
using GravityCTRL.FilterChili.Resolvers.Range;
using JetBrains.Annotations;

namespace GravityCTRL.FilterChili.Providers
{
    public class DecimalDomainProvider<TSource> : DomainProvider<TSource, decimal>
    {
        internal DecimalDomainProvider(Expression<Func<TSource, decimal>> selector) : base(selector) {}

        [UsedImplicitly]
        public DecimalRangeResolver<TSource> Range(string name)
        {
            return new DecimalRangeResolver<TSource>(name, Selector);
        }

        [UsedImplicitly]
        public DecimalComparisonResolver<TSource> GreaterThan(string name)
        {
            return new DecimalComparisonResolver<TSource>(name, new GreaterThanComparer<TSource, decimal>(decimal.MinValue), Selector);
        }

        [UsedImplicitly]
        public DecimalComparisonResolver<TSource> LessThan(string name)
        {
            return new DecimalComparisonResolver<TSource>(name, new LessThanComparer<TSource, decimal>(decimal.MaxValue), Selector);
        }

        [UsedImplicitly]
        public DecimalComparisonResolver<TSource> GreaterThanOrEqual(string name)
        {
            return new DecimalComparisonResolver<TSource>(name, new GreaterThanOrEqualComparer<TSource, decimal>(decimal.MinValue), Selector);
        }

        [UsedImplicitly]
        public DecimalComparisonResolver<TSource> LessThanOrEqual(string name)
        {
            return new DecimalComparisonResolver<TSource>(name, new LessThanOrEqualComparer<TSource, decimal>(decimal.MaxValue), Selector);
        }
    }
}