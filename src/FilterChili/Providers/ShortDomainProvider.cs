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
using GravityCTRL.FilterChili.Resolvers.Range;

namespace GravityCTRL.FilterChili.Providers
{
    public class ShortDomainProvider<TSource> : DomainProvider<TSource, short>
    {
        internal ShortDomainProvider(Expression<Func<TSource, short>> selector) : base(selector) {}

        public ShortRangeResolver<TSource> Range(string name, Action<ShortRangeResolver<TSource>> options = null)
        {
            var resolver = new ShortRangeResolver<TSource>(name, Selector);
            options?.Invoke(resolver);
            return resolver;
        }
    }
}