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
using GravityCTRL.FilterChili.Models;
using JetBrains.Annotations;

namespace GravityCTRL.FilterChili.Extensions
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> Append<T>(this T value, IEnumerable<T> values)
        {
            yield return value;
            foreach (var item in values)
            {
                yield return item;
            }
        }

        [NotNull]
        public static Option<Expression> Or([NotNull] this IEnumerable<Expression> expressions)
        {
            return CreateExpression(Expression.Or, expressions);
        }

        [NotNull]
        public static Option<Expression> And([NotNull] this IEnumerable<Expression> expressions)
        {
            return CreateExpression(Expression.AndAlso, expressions);
        }

        [NotNull]
        private static Option<Expression> CreateExpression(Func<Expression, Expression, BinaryExpression> binaryExpression, [NotNull] IEnumerable<Expression> expressions)
        {
            var expressionList = expressions as IList<Expression> ?? expressions.ToList();

            if (expressionList.Count == 0)
            {
                return Option.None<Expression>();
            }

            var expression = expressionList.First();
            if (expressionList.Count == 1)
            {
                return Option.Some(expression);
            }

            for (var index = 1; index < expressionList.Count; index++)
            {
                expression = binaryExpression(expressionList[index], expression);
            }

            return Option.Some(expression);
        }
    }
}
