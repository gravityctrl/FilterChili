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

namespace GravityCTRL.FilterChili.Resolvers
{
    public class IntRangeResolver<TSource> : RangeResolver<TSource, int>
    {
        internal IntRangeResolver(string name, Expression<Func<TSource, int>> selector) : base(name, selector, int.MinValue, int.MaxValue) { }

        protected override Expression<Func<TSource, bool>> FilterExpression()
        {
            if (SelectedRange.Min != int.MinValue && SelectedRange.Max != int.MaxValue)
            {
                var minConstant = Expression.Constant(SelectedRange.Min);
                var maxConstant = Expression.Constant(SelectedRange.Max);
                var greaterThanExpression = Expression.GreaterThanOrEqual(Selector.Body, minConstant);
                var lessThanExpression = Expression.LessThanOrEqual(Selector.Body, maxConstant);
                var andExpression = Expression.And(greaterThanExpression, lessThanExpression);
                return Expression.Lambda<Func<TSource, bool>>(andExpression, Selector.Parameters);
            }

            if (SelectedRange.Max != int.MaxValue)
            {
                var maxConstant = Expression.Constant(SelectedRange.Max);
                var lessThanExpression = Expression.LessThanOrEqual(Selector.Body, maxConstant);
                return Expression.Lambda<Func<TSource, bool>>(lessThanExpression, Selector.Parameters);
            }

            if (SelectedRange.Min != int.MinValue)
            {
                var minConstant = Expression.Constant(SelectedRange.Min);
                var greaterThanExpression = Expression.GreaterThanOrEqual(Selector.Body, minConstant);
                return Expression.Lambda<Func<TSource, bool>>(greaterThanExpression, Selector.Parameters);
            }

            return null;
        }
    }
}