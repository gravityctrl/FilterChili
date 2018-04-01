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
using JetBrains.Annotations;

namespace GravityCTRL.FilterChili.Selectors
{
    public sealed class StringFilterSelector<TSource> : FilterSelector<TSource, string>
    {
        internal StringFilterSelector(Expression<Func<TSource, string>> selector) : base(selector) {}

        [NotNull]
        [UsedImplicitly]
        public ListResolver<TSource, string> WithList()
        {
            var resolver = new ListResolver<TSource, string>(Selector);
            FilterResolver = resolver;
            return resolver;
        }

        [NotNull]
        [UsedImplicitly]
        public GroupResolver<TSource, string, TGroupIdentifier> WithGroup<TGroupIdentifier>([NotNull] Expression<Func<TSource, TGroupIdentifier>> groupSelector) where TGroupIdentifier : IComparable
        {
            var resolver = new GroupResolver<TSource, string, TGroupIdentifier>(Selector, groupSelector);
            FilterResolver = resolver;
            return resolver;
        }
    }
}