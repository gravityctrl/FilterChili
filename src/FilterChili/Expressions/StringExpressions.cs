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
using GravityCTRL.FilterChili.Extensions;
using GravityCTRL.FilterChili.Phonetics;
using JetBrains.Annotations;

namespace GravityCTRL.FilterChili.Expressions
{
    internal static class StringExpressions
    {
        [CanBeNull]
        public static Expression<Func<TSource, bool>> CreateFilterExpression<TSource>([NotNull] IReadOnlyList<string> values, StringComparisonStrategy strategy, Expression<Func<TSource, string>> selector)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (strategy)
            {
                case StringComparisonStrategy.Contains:
                {
                    var selectedValueExpressions = values.Select(Expression.Constant);
                    var containsExpressions = selectedValueExpressions.Select(expression => Expression.Call(selector.Body, MethodExpressions.StringContainsExpression, expression));
                    var orExpression = containsExpressions.Or();
                    return orExpression == null ? null : Expression.Lambda<Func<TSource, bool>>(orExpression, selector.Parameters);
                }
                case StringComparisonStrategy.Soundex:
                {
                    var compiledExpression = selector.Compile();
                    return entity => values.Select(Soundex.ToSoundex).Contains(compiledExpression(entity).ToSoundex());
                }
                case StringComparisonStrategy.GermanSoundex:
                {
                    var compiledExpression = selector.Compile();
                    return entity => values.Select(GermanSoundex.ToGermanSoundex).Contains(compiledExpression(entity).ToGermanSoundex());
                }
                default:
                {
                    var selectedValueExpressions = values.Select(Expression.Constant);
                    var equalsExpressions = selectedValueExpressions.Select(expression => Expression.Equal(expression, selector.Body));
                    var orExpression = equalsExpressions.Or();
                    return orExpression == null ? null : Expression.Lambda<Func<TSource, bool>>(orExpression, selector.Parameters);
                }
            }
        }
    }
}
