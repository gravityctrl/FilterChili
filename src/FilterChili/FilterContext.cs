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

using System.Collections.Generic;
using System.Linq;
using GravityCTRL.FilterChili.Resolvers;

namespace GravityCTRL.FilterChili
{
    public abstract class FilterContext<TSource>
    {
        private readonly ContextOptions<TSource> _contextOptions;

        protected FilterContext(IQueryable<TSource> queryable)
        {
            _contextOptions = new ContextOptions<TSource>(queryable, Configure);
        }

        public IQueryable<TSource> ApplyFilters()
        {
            return _contextOptions.ApplyFilters();
        }

        public IEnumerable<DomainResolver<TSource>> Domains()
        {
            return _contextOptions.Domains();
        }

        protected abstract void Configure(ContextOptions<TSource> options);
    }
}
