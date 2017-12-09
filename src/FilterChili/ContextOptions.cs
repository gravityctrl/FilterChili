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
using GravityCTRL.FilterChili.Selectors;
using JetBrains.Annotations;

namespace GravityCTRL.FilterChili
{
    public class ContextOptions<TSource>
    {
        private readonly IQueryable<TSource> _queryable;
        private readonly List<FilterSelector<TSource>> _filters;

        [UsedImplicitly]
        public bool EnableMars { get; set; }

        internal ContextOptions(IQueryable<TSource> queryable, Action<ContextOptions<TSource>> configure)
        {
            _queryable = queryable;
            _filters = new List<FilterSelector<TSource>>();
            configure(this);
        }

        public StringFilterSelector<TSource> Filter(Expression<Func<TSource, string>> valueSelector)
        {
            var filter = new StringFilterSelector<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        public IntFilterSelector<TSource> Filter(Expression<Func<TSource, int>> valueSelector)
        {
            var filter = new IntFilterSelector<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        #region Internal Methods

        internal FilterSelector<TSource> GetFilter(string name)
        {
            return _filters.SingleOrDefault(filter => filter.HasName(name));
        }

        internal IQueryable<TSource> ApplyFilters()
        {
            var queryable = _queryable.AsQueryable();
            return _filters.Aggregate(queryable, (current, filter) => filter.ApplyFilter(current));
        }

        internal async Task<IEnumerable<DomainResolver<TSource>>> Domains()
        {
            if (!_filters.Any(f => f.NeedsToBeResolved))
            {
                return _filters.Select(filter => filter.Domain());
            }

            if (EnableMars)
            {
                await ResolveConcurrently();
            }
            else
            {
                await Resolve();
            }

            return _filters.Select(filter => filter.Domain());
        }

        #endregion

        #region Private Methods

        private async Task Resolve()
        {
            var filters = _filters.ToList();
            for (var indexToResolve = 0; indexToResolve < filters.Count; indexToResolve++)
            {
                var currentFilter = filters[indexToResolve];
                if (!currentFilter.NeedsToBeResolved)
                {
                    continue;
                }

                var selectableItems = _queryable.AsQueryable();
                var ignoredIndex = indexToResolve;
                var filtersToExecute = filters.Where((filterSelector, indexToFilter) => indexToFilter != ignoredIndex);
                selectableItems = filtersToExecute.Aggregate(selectableItems, (current, filterSelector) => filterSelector.ApplyFilter(current));

                var filter = filters[indexToResolve];
                await filter.SetAvailableEntities(_queryable);
                await filter.SetSelectableEntities(selectableItems);
                currentFilter.NeedsToBeResolved = false;
            }
        }

        private async Task ResolveConcurrently()
        {
            var filters = _filters.ToList();

            IEnumerable<Task> CreateResolvingTasks(FilterSelector<TSource> currentFilter, int ignoredIndex)
            {
                var selectableItems = _queryable.AsQueryable();
                yield return currentFilter.SetAvailableEntities(selectableItems);

                var filtersToExecute = filters.Where((filterSelector, indexToFilter) => indexToFilter != ignoredIndex);
                selectableItems = filtersToExecute.Aggregate(selectableItems, (current, filterSelector) => filterSelector.ApplyFilter(current));
                yield return currentFilter.SetSelectableEntities(selectableItems);
            }

            var filtersToResolve = filters.Where(filter => filter.NeedsToBeResolved).ToList();
            var resolvingTasks = filtersToResolve.SelectMany(CreateResolvingTasks);

            await Task.WhenAll(resolvingTasks);

            filtersToResolve.ForEach(filter => filter.NeedsToBeResolved = false);
        }

        #endregion
    }
}