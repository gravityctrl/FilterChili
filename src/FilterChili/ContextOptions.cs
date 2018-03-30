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
using GravityCTRL.FilterChili.Search;
using GravityCTRL.FilterChili.Selectors;
using JetBrains.Annotations;

namespace GravityCTRL.FilterChili
{
    public sealed class ContextOptions<TSource>
    {
        private readonly IQueryable<TSource> _queryable;
        private readonly List<FilterSelector<TSource>> _filters;
        private readonly SearchResolver<TSource> _searchResolver;

        [UsedImplicitly]
        public CalculationStrategy CalculationStrategy { get; set; }

        internal ContextOptions(IQueryable<TSource> queryable, [NotNull] Action<ContextOptions<TSource>> configure)
        {
            _queryable = queryable;
            _filters = new List<FilterSelector<TSource>>();
            _searchResolver = new SearchResolver<TSource>();
            configure(this);
        }

        #region Filters

        [NotNull]
        [UsedImplicitly]
        public SearchSelector<TSource> Search([NotNull] Expression<Func<TSource, string>> valueSelector)
        {
            var searcher = new SearchSelector<TSource>(valueSelector);
            _searchResolver.AddSearcher(searcher);
            return searcher;
        }

        [NotNull]
        [UsedImplicitly]
        public ByteFilterSelector<TSource> Filter([NotNull] Expression<Func<TSource, byte>> valueSelector)
        {
            var filter = new ByteFilterSelector<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        [NotNull]
        [UsedImplicitly]
        public CharFilterSelector<TSource> Filter([NotNull] Expression<Func<TSource, char>> valueSelector)
        {
            var filter = new CharFilterSelector<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        [NotNull]
        [UsedImplicitly]
        public DecimalFilterSelector<TSource> Filter([NotNull] Expression<Func<TSource, decimal>> valueSelector)
        {
            var filter = new DecimalFilterSelector<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        [NotNull]
        [UsedImplicitly]
        public DoubleFilterSelector<TSource> Filter([NotNull] Expression<Func<TSource, double>> valueSelector)
        {
            var filter = new DoubleFilterSelector<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        [NotNull]
        [UsedImplicitly]
        public FloatFilterSelector<TSource> Filter([NotNull] Expression<Func<TSource, float>> valueSelector)
        {
            var filter = new FloatFilterSelector<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        [NotNull]
        [UsedImplicitly]
        public IntFilterSelector<TSource> Filter([NotNull] Expression<Func<TSource, int>> valueSelector)
        {
            var filter = new IntFilterSelector<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        [NotNull]
        [UsedImplicitly]
        public LongFilterSelector<TSource> Filter([NotNull] Expression<Func<TSource, long>> valueSelector)
        {
            var filter = new LongFilterSelector<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        [NotNull]
        [UsedImplicitly]
        public SByteFilterSelector<TSource> Filter([NotNull] Expression<Func<TSource, sbyte>> valueSelector)
        {
            var filter = new SByteFilterSelector<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        [NotNull]
        [UsedImplicitly]
        public ShortFilterSelector<TSource> Filter([NotNull] Expression<Func<TSource, short>> valueSelector)
        {
            var filter = new ShortFilterSelector<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        [NotNull]
        [UsedImplicitly]
        public StringFilterSelector<TSource> Filter([NotNull] Expression<Func<TSource, string>> valueSelector)
        {
            var filter = new StringFilterSelector<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        [NotNull]
        [UsedImplicitly]
        public UIntFilterSelector<TSource> Filter([NotNull] Expression<Func<TSource, uint>> valueSelector)
        {
            var filter = new UIntFilterSelector<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        [NotNull]
        [UsedImplicitly]
        public ULongFilterSelector<TSource> Filter([NotNull] Expression<Func<TSource, ulong>> valueSelector)
        {
            var filter = new ULongFilterSelector<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        [NotNull]
        [UsedImplicitly]
        public UShortFilterSelector<TSource> Filter([NotNull] Expression<Func<TSource, ushort>> valueSelector)
        {
            var filter = new UShortFilterSelector<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        #endregion

        #region Internal Methods

        [CanBeNull]
        internal FilterSelector<TSource> GetFilter(string name)
        {
            return _filters.SingleOrDefault(filter => filter.HasName(name));
        }

        internal IQueryable<TSource> ApplyFilters()
        {
            var queryable = _searchResolver.ApplySearch(_queryable);
            return _filters.Aggregate(queryable, (current, filter) => filter.ApplyFilter(current));
        }

        [ItemNotNull]
        internal async Task<IEnumerable<DomainResolver<TSource>>> Domains()
        {
            return await Domains(CalculationStrategy);
        }

        [ItemNotNull]
        internal async Task<IEnumerable<DomainResolver<TSource>>> Domains(CalculationStrategy calculationStrategy)
        {
            if (!_filters.Any(f => f.NeedsToBeResolved))
            {
                return _filters.Select(filter => filter.Domain());
            }

            await Resolve(calculationStrategy);
            return _filters.Select(filter => filter.Domain());
        }

        internal void SetSearch(string search)
        {
            _searchResolver.SetSearchString(search);
        }

        #endregion

        #region Private Methods

        private async Task Resolve(CalculationStrategy calculationStrategy)
        {
            var queryable = _searchResolver.ApplySearch(_queryable);

            // Check if this is unnecessary.
            async Task ResolveFilterAtIndex(int ignoredIndex)
            {
                var currentFilter = _filters[ignoredIndex];
                var selectableItems = queryable.AsQueryable();
                if (currentFilter.NeedsToBeResolved)
                {
                    await currentFilter.SetAvailableEntities(selectableItems);
                }

                if (calculationStrategy == CalculationStrategy.Full)
                {
                    var filtersToExecute = _filters.Where((filterSelector, indexToFilter) => indexToFilter != ignoredIndex);
                    selectableItems = filtersToExecute.Aggregate(selectableItems, (current, filterSelector) => filterSelector.ApplyFilter(current));
                    await currentFilter.SetSelectableEntities(selectableItems);
                }

                currentFilter.NeedsToBeResolved = false;
            }

            for (var i = 0; i < _filters.Count; i++)
            {
                await ResolveFilterAtIndex(i);
            }
        }

        #endregion
    }
}