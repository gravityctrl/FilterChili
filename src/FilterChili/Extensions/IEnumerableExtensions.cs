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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace GravityCTRL.FilterChili.Extensions
{
    internal static class EnumerableExtensions
    {
        [CanBeNull]
        public static Expression Or(this IEnumerable<Expression> expressions)
        {
            return CreateExpression(Expression.Or, expressions);
        }

        [CanBeNull]
        public static Expression And(this IEnumerable<Expression> expressions)
        {
            return CreateExpression(Expression.And, expressions);
        }

        [CanBeNull]
        private static Expression CreateExpression(Func<Expression, Expression, BinaryExpression> binaryExpression, IEnumerable<Expression> expressions)
        {
            var expressionList = expressions as IList<Expression> ?? expressions.ToList();

            if (expressionList.Count == 0)
            {
                return null;
            }

            var expression = expressionList[0];
            if (expressionList.Count == 1)
            {
                return expression;
            }

            for (var index = 1; index < expressionList.Count; index++)
            {
                expression = binaryExpression(expression, expressionList[index]);
            }

            return expression;
        }
    }
}
