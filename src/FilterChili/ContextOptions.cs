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
        public CalculationStrategy CalculationStrategy { get; set; }

        internal ContextOptions(IQueryable<TSource> queryable, Action<ContextOptions<TSource>> configure)
        {
            _queryable = queryable;
            _filters = new List<FilterSelector<TSource>>();
            configure(this);
        }

        #region Filters

        public ByteFilterSelector<TSource> Filter(Expression<Func<TSource, byte>> valueSelector)
        {
            var filter = new ByteFilterSelector<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        public CharFilterSelector<TSource> Filter(Expression<Func<TSource, char>> valueSelector)
        {
            var filter = new CharFilterSelector<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        public DecimalFilterSelector<TSource> Filter(Expression<Func<TSource, decimal>> valueSelector)
        {
            var filter = new DecimalFilterSelector<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        public DoubleFilterSelector<TSource> Filter(Expression<Func<TSource, double>> valueSelector)
        {
            var filter = new DoubleFilterSelector<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        public FloatFilterSelector<TSource> Filter(Expression<Func<TSource, float>> valueSelector)
        {
            var filter = new FloatFilterSelector<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        public IntFilterSelector<TSource> Filter(Expression<Func<TSource, int>> valueSelector)
        {
            var filter = new IntFilterSelector<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        public LongFilterSelector<TSource> Filter(Expression<Func<TSource, long>> valueSelector)
        {
            var filter = new LongFilterSelector<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        public SByteFilterSelector<TSource> Filter(Expression<Func<TSource, sbyte>> valueSelector)
        {
            var filter = new SByteFilterSelector<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        public ShortFilterSelector<TSource> Filter(Expression<Func<TSource, short>> valueSelector)
        {
            var filter = new ShortFilterSelector<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        public StringFilterSelector<TSource> Filter(Expression<Func<TSource, string>> valueSelector)
        {
            var filter = new StringFilterSelector<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        public UIntFilterSelector<TSource> Filter(Expression<Func<TSource, uint>> valueSelector)
        {
            var filter = new UIntFilterSelector<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        public ULongFilterSelector<TSource> Filter(Expression<Func<TSource, ulong>> valueSelector)
        {
            var filter = new ULongFilterSelector<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        public UShortFilterSelector<TSource> Filter(Expression<Func<TSource, ushort>> valueSelector)
        {
            var filter = new UShortFilterSelector<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        #endregion

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
            return await Domains(CalculationStrategy);
        }

        internal async Task<IEnumerable<DomainResolver<TSource>>> Domains(CalculationStrategy calculationStrategy)
        {
            if (!_filters.Any(f => f.NeedsToBeResolved))
            {
                return _filters.Select(filter => filter.Domain());
            }

            await Resolve(calculationStrategy);
            return _filters.Select(filter => filter.Domain());
        }

        #endregion

        #region Private Methods

        private async Task Resolve(CalculationStrategy calculationStrategy)
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

                if (calculationStrategy == CalculationStrategy.Full)
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

        #endregion
    }
}