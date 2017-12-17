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

using GravityCTRL.FilterChili.Phonetics;
using System;
using System.Linq;
using System.Linq.Expressions;
using GravityCTRL.FilterChili.Enums;
using GravityCTRL.FilterChili.Expressions;
using JetBrains.Annotations;
using static GravityCTRL.FilterChili.Expressions.MethodExpressions;

namespace GravityCTRL.FilterChili.Resolvers
{
    public class StringListResolver<TSource> : ListResolver<TSource, string>
    {
        internal StringListResolver(string name, Expression<Func<TSource, string>> selector) : base(name, selector) { }

        [UsedImplicitly]
        public StringComparisonStrategy ComparisonStrategy { get; set; }

        protected override Expression<Func<TSource, bool>> FilterExpression()
        {
            if (!SelectedValues.Any())
            {
                return null;
            }

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (ComparisonStrategy)
            {
                case StringComparisonStrategy.Equals:
                {
                    var selectedValueExpressions = SelectedValues.Select(Expression.Constant);
                    var equalsExpressions = selectedValueExpressions.Select(expression => Expression.Equal(expression, Selector.Body));
                    var orExpression = equalsExpressions.Or();
                    return orExpression == null ? null : Expression.Lambda<Func<TSource, bool>>(orExpression, Selector.Parameters);
                }
                case StringComparisonStrategy.Contains:
                {
                    var selectedValueExpressions = SelectedValues.Select(Expression.Constant);
                    var equalsExpressions = selectedValueExpressions.Select(expression => Expression.Call(expression, StringContainsExpression, Selector.Body));
                    var orExpression = equalsExpressions.Or();
                    return orExpression == null ? null : Expression.Lambda<Func<TSource, bool>>(orExpression, Selector.Parameters);
                }
                case StringComparisonStrategy.Soundex:
                {
                    var compiledExpression = Selector.Compile();
                    return entity => SelectedValues.Select(Soundex.ToSoundex).Contains(compiledExpression(entity).ToSoundex());
                }
                case StringComparisonStrategy.GermanSoundex:
                {
                    var compiledExpression = Selector.Compile();
                    return entity => SelectedValues.Select(GermanSoundex.ToGermanSoundex).Contains(compiledExpression(entity).ToGermanSoundex());
                }
                default:
                {
                    return null;
                }
            }
        }
    }
}