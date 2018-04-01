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
using GravityCTRL.FilterChili.Exceptions;
using GravityCTRL.FilterChili.Models;
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

        internal ContextOptions(IQueryable<TSource> queryable, [NotNull] Action<ContextOptions<TSource>> configure)
        {
            _queryable = queryable;
            _filters = new List<FilterSelector<TSource>>();
            _searchResolver = new SearchResolver<TSource>();
            configure(this);
            CheckPrerequisites();
        }

        #region Filters

        [NotNull]
        [UsedImplicitly]
        public SearchSpecification<TSource> Search([NotNull] Expression<Func<TSource, string>> valueSelector)
        {
            var searcher = new SearchSpecification<TSource>(valueSelector);
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
            return await CalculateDomains(Option.None<CalculationStrategy>());
        }

        [ItemNotNull]
        internal async Task<IEnumerable<DomainResolver<TSource>>> Domains(CalculationStrategy calculationStrategy)
        {
            return await CalculateDomains(Option.Some(calculationStrategy));
        }

        internal void SetSearch(string search)
        {
            _searchResolver.SetSearchString(search);
        }

        #endregion

        #region Private Methods

        private void CheckPrerequisites()
        {
            var invalidFilter = _filters.FirstOrDefault(filter => !filter.HasDomainResolver);
            if (invalidFilter != null)
            {
                throw new MissingResolverException(invalidFilter.Name);
            }
        }

        [ItemNotNull]
        private async Task<IEnumerable<DomainResolver<TSource>>> CalculateDomains([NotNull] Option<CalculationStrategy> calculationStrategy)
        {
            if (!_filters.Any(f => f.NeedsToBeResolved))
            {
                return _filters.Select(filter => filter.Domain());
            }

            await Resolve(calculationStrategy);
            return _filters.Select(filter => filter.Domain());
        }

        private async Task Resolve([NotNull] Option<CalculationStrategy> calculationStrategy)
        {
            var queryable = _searchResolver.ApplySearch(_queryable);
            for (var index = 0; index < _filters.Count; index++)
            {
                await ResolveFilterAtIndex(queryable, calculationStrategy, index);
            }
        }

        private async Task ResolveFilterAtIndex([NotNull] IQueryable<TSource> queryable, [NotNull] Option<CalculationStrategy> calculationStrategy, int index)
        {
            var currentFilter = _filters[index];
            var usedCalculationStrategy = calculationStrategy.TryGetValue(out var value) ? value : currentFilter.CalculationStrategy;

            // ReSharper disable once ImplicitlyCapturedClosure
            Option<IQueryable<TSource>> CreateAllEntitiesOption()
            {
                return ShouldSetAvailableItems(usedCalculationStrategy) && currentFilter.NeedsToBeResolved
                    ? Option.Some(queryable)
                    : Option.None<IQueryable<TSource>>();
            }

            // ReSharper disable once ImplicitlyCapturedClosure
            Option<IQueryable<TSource>> CreateSelectableEntitiesOption()
            {
                IQueryable<TSource> CreateFilterAggregate()
                {
                    var filtersToExecute = _filters.Where((filterSelector, indexToFilter) => indexToFilter != index);
                    var aggregate = filtersToExecute.Aggregate(queryable, (current, filterSelector) => filterSelector.ApplyFilter(current));
                    return aggregate;
                }

                return ShouldSetSelectableItems(usedCalculationStrategy)
                    ? Option.Some(CreateFilterAggregate())
                    : Option.None<IQueryable<TSource>>();
            }

            await currentFilter.SetEntities(CreateAllEntitiesOption(), CreateSelectableEntitiesOption());
            currentFilter.NeedsToBeResolved = false;
        }

        private static bool ShouldSetAvailableItems(CalculationStrategy calculationStrategy)
        {
            return (calculationStrategy & CalculationStrategy.AvailableValues) == CalculationStrategy.AvailableValues;
        }

        private static bool ShouldSetSelectableItems(CalculationStrategy calculationStrategy)
        {
            return (calculationStrategy & CalculationStrategy.SelectableValues) == CalculationStrategy.SelectableValues;
        }

        #endregion
    }
}