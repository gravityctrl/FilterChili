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

namespace GravityCTRL.FilterChili.Comparison
{
    internal sealed class GreaterThanComparer<TSource, TValue> : Comparer<TSource, TValue> where TValue : IComparable
    {
        private readonly TValue _minValue;

        public override string FilterType { get; } = "GreaterThan";

        public GreaterThanComparer(TValue minValue)
        {
            _minValue = minValue;
        }

        public override Expression<Func<TSource, bool>> FilterExpression(Expression<Func<TSource, TValue>> selector, TValue selectedValue)
        {
            if (_minValue.CompareTo(selectedValue) >= 0)
            {
                return null;
            }

            var valueConstant = Expression.Constant(selectedValue);
            var expression = Expression.GreaterThan(selector.Body, valueConstant);
            return Expression.Lambda<Func<TSource, bool>>(expression, selector.Parameters);
        }
    }
}
