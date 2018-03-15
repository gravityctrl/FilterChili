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
using GravityCTRL.FilterChili.Providers;
using GravityCTRL.FilterChili.Resolvers;
using JetBrains.Annotations;

namespace GravityCTRL.FilterChili
{
    public class ContextOptions<TSource>
    {
        private readonly IQueryable<TSource> _queryable;
        private readonly List<DomainProvider<TSource>> _filters;

        [UsedImplicitly]
        public CalculationStrategy CalculationStrategy { get; set; }

        internal ContextOptions(IQueryable<TSource> queryable, Action<ContextOptions<TSource>> configure)
        {
            _queryable = queryable;
            _filters = new List<DomainProvider<TSource>>();
            configure(this);
        }

        #region Filters

        [UsedImplicitly]
        public ByteDomainProvider<TSource> Filter(Expression<Func<TSource, byte>> valueSelector)
        {
            var filter = new ByteDomainProvider<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        [UsedImplicitly]
        public CharDomainProvider<TSource> Filter(Expression<Func<TSource, char>> valueSelector)
        {
            var filter = new CharDomainProvider<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        [UsedImplicitly]
        public DecimalDomainProvider<TSource> Filter(Expression<Func<TSource, decimal>> valueSelector)
        {
            var filter = new DecimalDomainProvider<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        [UsedImplicitly]
        public DoubleDomainProvider<TSource> Filter(Expression<Func<TSource, double>> valueSelector)
        {
            var filter = new DoubleDomainProvider<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        [UsedImplicitly]
        public FloatDomainProvider<TSource> Filter(Expression<Func<TSource, float>> valueSelector)
        {
            var filter = new FloatDomainProvider<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        [UsedImplicitly]
        public IntDomainProvider<TSource> Filter(Expression<Func<TSource, int>> valueSelector)
        {
            var filter = new IntDomainProvider<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        [UsedImplicitly]
        public LongDomainProvider<TSource> Filter(Expression<Func<TSource, long>> valueSelector)
        {
            var filter = new LongDomainProvider<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        [UsedImplicitly]
        public SByteDomainProvider<TSource> Filter(Expression<Func<TSource, sbyte>> valueSelector)
        {
            var filter = new SByteDomainProvider<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        [UsedImplicitly]
        public ShortDomainProvider<TSource> Filter(Expression<Func<TSource, short>> valueSelector)
        {
            var filter = new ShortDomainProvider<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        [UsedImplicitly]
        public StringDomainProvider<TSource> Filter(Expression<Func<TSource, string>> valueSelector)
        {
            var filter = new StringDomainProvider<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        [UsedImplicitly]
        public UIntDomainProvider<TSource> Filter(Expression<Func<TSource, uint>> valueSelector)
        {
            var filter = new UIntDomainProvider<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        [UsedImplicitly]
        public ULongDomainProvider<TSource> Filter(Expression<Func<TSource, ulong>> valueSelector)
        {
            var filter = new ULongDomainProvider<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        [UsedImplicitly]
        public UShortDomainProvider<TSource> Filter(Expression<Func<TSource, ushort>> valueSelector)
        {
            var filter = new UShortDomainProvider<TSource>(valueSelector);
            _filters.Add(filter);
            return filter;
        }

        #endregion

        #region Internal Methods

        internal DomainProvider<TSource> GetFilter(string name)
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
                var selectableItems = _queryable.AsQueryable();
                if (currentFilter.NeedsToBeResolved)
                {
                    await currentFilter.SetAvailableEntities(selectableItems);
                }

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