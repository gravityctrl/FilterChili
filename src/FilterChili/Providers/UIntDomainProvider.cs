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
    public class UIntDomainProvider<TSource> : DomainProvider<TSource, uint>
    {
        internal UIntDomainProvider(Expression<Func<TSource, uint>> selector) : base(selector) {}

        [UsedImplicitly]
        public UIntRangeResolver<TSource> Range(string name)
        {
            return new UIntRangeResolver<TSource>(name, Selector);
        }

        [UsedImplicitly]
        public UIntComparisonResolver<TSource> GreaterThan(string name)
        {
            return new UIntComparisonResolver<TSource>(name, new GreaterThanComparer<TSource, uint>(uint.MinValue), Selector);
        }

        [UsedImplicitly]
        public UIntComparisonResolver<TSource> LessThan(string name)
        {
            return new UIntComparisonResolver<TSource>(name, new LessThanComparer<TSource, uint>(uint.MaxValue), Selector);
        }

        [UsedImplicitly]
        public UIntComparisonResolver<TSource> GreaterThanOrEqual(string name)
        {
            return new UIntComparisonResolver<TSource>(name, new GreaterThanOrEqualComparer<TSource, uint>(uint.MinValue), Selector);
        }

        [UsedImplicitly]
        public UIntComparisonResolver<TSource> LessThanOrEqual(string name)
        {
            return new UIntComparisonResolver<TSource>(name, new LessThanOrEqualComparer<TSource, uint>(uint.MaxValue), Selector);
        }
    }
}