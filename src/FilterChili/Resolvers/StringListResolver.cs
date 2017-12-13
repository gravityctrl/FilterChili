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
using JetBrains.Annotations;

namespace GravityCTRL.FilterChili.Resolvers
{
    public class StringListResolver<TSource> : ListResolver<TSource, string>
    {
        internal StringListResolver(string name, Expression<Func<TSource, string>> selector) : base(name, selector) { }

        [UsedImplicitly]
        public StringComparisonStrategy ComparisonStrategy { get; set; }

        protected override Expression<Func<IGrouping<string, TSource>, bool>> FilterExpression()
        {
            if (!SelectedValues.Any())
            {
                return null;
            }

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (ComparisonStrategy)
            {
                case StringComparisonStrategy.Contains:
                {
                    return group => SelectedValues.Any(value => value.Contains(group.Key));
                }
                case StringComparisonStrategy.Soundex:
                {
                    return group => SelectedValues.Select(Soundex.ToSoundex).Contains(group.Key.ToSoundex());
                }
                case StringComparisonStrategy.GermanSoundex:
                {
                    return group => SelectedValues.Select(Soundex.ToSoundex).Contains(group.Key.ToGermanSoundex());
                }
                default:
                {
                    return group => SelectedValues.Contains(group.Key);
                }
            }
        }
    }
}