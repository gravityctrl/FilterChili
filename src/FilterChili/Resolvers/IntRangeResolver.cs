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
using System.Linq;
using System.Linq.Expressions;

namespace GravityCTRL.FilterChili.Resolvers
{
    public class IntRangeResolver<TSource> : RangeResolver<TSource, int>
    {
        internal IntRangeResolver(string name, Expression<Func<TSource, int>> selector) : base(name, selector, int.MinValue, int.MaxValue) { }

        protected override Expression<Func<IGrouping<int, TSource>, bool>> FilterExpression()
        {
            if (SelectedRange.Min != int.MinValue && SelectedRange.Max != int.MaxValue)
            {
                return group => group.Key >= SelectedRange.Min && group.Key <= SelectedRange.Max;
            }

            if (SelectedRange.Min == int.MinValue)
            {
                return group => group.Key <= SelectedRange.Max;
            }

            if (SelectedRange.Max == int.MaxValue)
            {
                return group => group.Key >= SelectedRange.Min;
            }

            return null;
        }
    }
}