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
using GravityCTRL.FilterChili.Extensions;
using GravityCTRL.FilterChili.Models;
using GravityCTRL.FilterChili.Resolvers;
using GravityCTRL.FilterChili.Resolvers.Interfaces;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace GravityCTRL.FilterChili
{
    public sealed class ListResolver<TSource, TSelector> 
        : DomainResolver<ListResolver<TSource, TSelector>, TSource, TSelector>, IListResolver<TSelector>
            where TSelector : IComparable
    {
        private bool _needsToBeResolved;

        internal override bool NeedsToBeResolved
        {
            get => _needsToBeResolved;
            set => _needsToBeResolved = value;
        }

        public override string FilterType { get; } = "List";

        [NotNull]
        internal IReadOnlyList<TSelector> SelectedValues { get; private set; }

        [CanBeNull]
        private IReadOnlyList<TSelector> _selectableValues;

        [CanBeNull]
        private IReadOnlyList<TSelector> _availableValues;

        [NotNull]
        [UsedImplicitly]
        public IReadOnlyList<Item<TSelector>> Values => CombineLists();

        public ListResolver([NotNull] Expression<Func<TSource, TSelector>> selector) : base(selector)
        {
            _needsToBeResolved = true;
            SelectedValues = new List<TSelector>();
        }

        #region Public Methods

        [UsedImplicitly]
        public void Set([NotNull] IEnumerable<TSelector> selectedValues)
        {
            SelectedValues = selectedValues as IReadOnlyList<TSelector> ?? selectedValues.ToList();
            _selectableValues = null;
            _needsToBeResolved = true;
        }

        [UsedImplicitly]
        public void Set(params TSelector[] selectedValues)
        {
            SelectedValues = selectedValues as IReadOnlyList<TSelector> ?? selectedValues.ToList();
            _selectableValues = null;
            _needsToBeResolved = true;
        }

        #endregion

        #region Public Overrides

        [UsedImplicitly]
        public override bool TrySet([CanBeNull] JToken domainToken)
        {
            var valuesToken = domainToken?.SelectToken("values");
            if (valuesToken == null)
            {
                return false;
            }

            var values = valuesToken.Values<TSelector>();
            Set(values);
            return true;
        }

        #endregion

        #region Internal Methods

        protected override Expression<Func<TSource, bool>> FilterExpression()
        {
            if (!SelectedValues.Any())
            {
                return null;
            }

            var selectedValueExpressions = SelectedValues.Select(selector => Expression.Constant(selector));
            var equalsExpressions = selectedValueExpressions.Select(expression => Expression.Equal(expression, Selector.Body));
            var orExpression = equalsExpressions.Or();
            return orExpression == null ? null : Expression.Lambda<Func<TSource, bool>>(orExpression, Selector.Parameters);
        }

        internal override async Task SetEntities(Option<IQueryable<TSource>> allEntities, Option<IQueryable<TSource>> selectableEntities)
        {
            if (allEntities.TryGetValue(out var all))
            {
                _availableValues = await CreateSelectorList(all);
            }

            if (selectableEntities.TryGetValue(out var selectable))
            {
                _selectableValues = await CreateSelectorList(selectable);
            }
        }

        #endregion

        #region Private Methods

        [NotNull]
        private async Task<IReadOnlyList<TSelector>> CreateSelectorList([NotNull] IQueryable<TSource> queryable)
        {
            return queryable is IAsyncEnumerable<TSource>
                ? await queryable.Select(Selector).Distinct().ToListAsync()
                : queryable.Select(Selector).Distinct().ToList();
        }

        [NotNull]
        private IReadOnlyList<Item<TSelector>> CombineLists()
        {
            Dictionary<TSelector, Item<TSelector>> entities;
            if (_availableValues != null)
            {
                entities = CreateDictionary(_availableValues, false);
                if (_selectableValues != null)
                {
                    SetSelectableStatus(_selectableValues, entities);
                }
            }
            else if (_selectableValues != null)
            {
                entities = CreateDictionary(_selectableValues, true);
            }
            else
            {
                return SelectedValues.Select(value => new Item<TSelector> { Value = value, IsSelected = true }).ToList();
            }

            SetSelectedStatus(SelectedValues, entities);

            return entities.Values.ToList();
        }

        [NotNull]
        private static Dictionary<TSelector, Item<TSelector>> CreateDictionary([NotNull] IEnumerable<TSelector> values, bool canBeSelected)
        {
            return values.ToDictionary(value => value, value => new Item<TSelector> { Value = value, CanBeSelected = canBeSelected });
        }

        private static void SetSelectedStatus([NotNull] IEnumerable<TSelector> selectedValues, IReadOnlyDictionary<TSelector, Item<TSelector>> dictionary)
        {
            foreach (var selectedValue in selectedValues)
            {
                if (dictionary.TryGetValue(selectedValue, out var selectable))
                {
                    selectable.IsSelected = true;
                }
            }
        }

        private static void SetSelectableStatus([NotNull] IEnumerable<TSelector> selectableValues, [NotNull] Dictionary<TSelector, Item<TSelector>> dictionary)
        {
            foreach (var selectable in dictionary.Values)
            {
                selectable.CanBeSelected = false;
            }

            foreach (var selectableValue in selectableValues)
            {
                if (dictionary.TryGetValue(selectableValue, out var selectable))
                {
                    selectable.CanBeSelected = true;
                }
            }
        }

        #endregion
    }
}