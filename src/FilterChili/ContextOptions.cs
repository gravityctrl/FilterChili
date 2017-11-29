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
using System.Threading.Tasks;
using GravityCTRL.FilterChili.Resolvers;

namespace GravityCTRL.FilterChili
{
    public class ContextOptions<TSource>
    {
        private readonly IQueryable<TSource> _queryable;
        private readonly List<FilterSelector<TSource>> _filters;

        internal ContextOptions(IQueryable<TSource> queryable, Action<ContextOptions<TSource>> configure)
        {
            _queryable = queryable;
            _filters = new List<FilterSelector<TSource>>();
            configure(this);
            Resolve().Wait();
        }

        public StringFilterSelector<TSource> Filter(Expression<Func<TSource, string>> valueSelector)
        {
            var filter = new StringFilterSelector<TSource>(async () => await Resolve(), valueSelector);
            _filters.Add(filter);
            return filter;
        }

        public IntFilterSelector<TSource> Filter(Expression<Func<TSource, int>> valueSelector)
        {
            var filter = new IntFilterSelector<TSource>(async () => await Resolve(), valueSelector);
            _filters.Add(filter);
            return filter;
        }

        public bool TrySet<TSelector>(string name, TSelector min, TSelector max)
        {
            return _filters.Any(filter => filter.TrySet(name, min, max));
        }

        #region Internal Methods

        internal IQueryable<TSource> ApplyFilters()
        {
            var queryable = _queryable.AsQueryable();
            return _filters.Aggregate(queryable, (current, filter) => filter.ApplyFilter(current));
        }

        internal IEnumerable<DomainResolver<TSource>> Domains()
        {
            return _filters.Select(filter => filter.Domain());
        }

        #endregion

        #region Private Methods

        private async Task Resolve()
        {
            var filters = _filters.ToList();
            for (var indexToResolve = 0; indexToResolve < filters.Count; indexToResolve++)
            {
                var selectableItems = _queryable.AsQueryable();

                var ignoredIndex = indexToResolve;
                var filtersToExecute = filters.Where((filterSelector, indexToFilter) => indexToFilter != ignoredIndex);
                selectableItems = filtersToExecute.Aggregate(selectableItems, (current, filter) => filter.ApplyFilter(current));

                await filters[indexToResolve].Resolve(_queryable, selectableItems);
            }
        }

        #endregion
    }
}