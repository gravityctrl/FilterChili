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
using GravityCTRL.FilterChili.Expressions;

namespace GravityCTRL.FilterChili.Search.ExpressionProviders
{
    internal sealed class EqualsExpressionProvider<TSource> : IExpressionProvider<TSource>
    {
        public bool AcceptsMultipleSearchInputs { get; } = false;

        public Expression SearchExpression(Expression<Func<TSource, string>> searchSelector, string search)
        {
            var constant = Expression.Constant(search);
            return Expression.Equal(Expression.Call(searchSelector.Body, MethodExpressions.ToLowerExpression), constant);
        }
    }
}