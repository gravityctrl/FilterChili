﻿// This file is part of FilterChili.
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
using GravityCTRL.FilterChili.Enums;
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

        [UsedImplicitly]
        public CalculationStrategy CalculationStrategy { get; set; }

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
            async Task ResolveFilterAtIndex(int ignoredIndex)
            {
                var currentFilter = filters[ignoredIndex];
                if (!currentFilter.NeedsToBeResolved)
                {
                    return;
                }

                var selectableItems = _queryable.AsQueryable();
                await currentFilter.SetAvailableEntities(selectableItems);

                if (CalculationStrategy == CalculationStrategy.Full)
                {
                    var filtersToExecute = filters.Where((filterSelector, indexToFilter) => indexToFilter != ignoredIndex);
                    selectableItems = filtersToExecute.Aggregate(selectableItems, (current, filterSelector) => filterSelector.ApplyFilter(current));
                    await currentFilter.SetSelectableEntities(selectableItems);
                }

                currentFilter.NeedsToBeResolved = false;
            }

            for (var i = 0; i < filters.Count; i++)
            {
                await ResolveFilterAtIndex(i);
            }
        }

        private async Task ResolveConcurrently()
        {
            var filters = _filters.ToList();
            IEnumerable<Task> CreateResolvingTasks(FilterSelector<TSource> currentFilter, int ignoredIndex)
            {
                if (!currentFilter.NeedsToBeResolved)
                {
                    yield break;
                }

                var selectableItems = _queryable.AsQueryable();
                yield return currentFilter.SetAvailableEntities(selectableItems);

                if (CalculationStrategy == CalculationStrategy.Full)
                {
                    var filtersToExecute = filters.Where((filterSelector, indexToFilter) => indexToFilter != ignoredIndex);
                    selectableItems = filtersToExecute.Aggregate(selectableItems, (current, filterSelector) => filterSelector.ApplyFilter(current));
                    yield return currentFilter.SetSelectableEntities(selectableItems);
                }
            }

            await Task.WhenAll(filters.SelectMany(CreateResolvingTasks));
            filters.ForEach(filter => filter.NeedsToBeResolved = false);
        }

        #endregion
    }
}