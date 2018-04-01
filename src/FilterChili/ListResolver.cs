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
    public sealed class ListResolver<TSource, TValue> 
        : FilterResolver<ListResolver<TSource, TValue>, TSource, TValue>, IListResolver<TValue>
            where TValue : IComparable
    {
        internal override bool NeedsToBeResolved { get; set; }

        public override string FilterType { get; } = "List";

        [NotNull]
        internal IReadOnlyList<TValue> SelectedValues { get; private set; }

        [NotNull]
        private Option<IReadOnlyList<TValue>> _selectableValues;

        [NotNull]
        private Option<IReadOnlyList<TValue>> _availableValues;

        [NotNull]
        [UsedImplicitly]
        public IReadOnlyList<Item<TValue>> Values => CombineLists();

        #region Constructors

        internal ListResolver([NotNull] Expression<Func<TSource, TValue>> selector) : base(selector)
        {
            NeedsToBeResolved = true;
            SelectedValues = new List<TValue>();
            _selectableValues = Option.None<IReadOnlyList<TValue>>();
            _availableValues = Option.None<IReadOnlyList<TValue>>();
        }

        #endregion

        #region Public Methods

        [UsedImplicitly]
        public void Set([NotNull] IEnumerable<TValue> selectedValues)
        {
            SelectedValues = selectedValues as IReadOnlyList<TValue> ?? selectedValues.ToList();
            _selectableValues = null;
            NeedsToBeResolved = true;
        }

        [UsedImplicitly]
        public void Set(params TValue[] selectedValues)
        {
            SelectedValues = selectedValues as IReadOnlyList<TValue> ?? selectedValues.ToList();
            _selectableValues = null;
            NeedsToBeResolved = true;
        }

        #endregion

        #region Public Overrides

        [UsedImplicitly]
        public override bool TrySet([CanBeNull] JToken filterToken)
        {
            var valuesToken = filterToken?.SelectToken("values");
            if (valuesToken == null)
            {
                return false;
            }

            var values = valuesToken.Values<TValue>();
            Set(values);
            return true;
        }

        #endregion

        #region Internal Methods

        protected override Option<Expression<Func<TSource, bool>>> FilterExpression()
        {
            if (!SelectedValues.Any())
            {
                return Option.None<Expression<Func<TSource, bool>>>();
            }

            var selectedValueExpressions = SelectedValues.Select(selector => Expression.Constant(selector));
            var equalsExpressions = selectedValueExpressions.Select(expression => Expression.Equal(expression, Selector.Body));
            var orExpression = equalsExpressions.Or();
            return orExpression != null 
                ? Option.Some(Expression.Lambda<Func<TSource, bool>>(orExpression, Selector.Parameters))
                : Option.None<Expression<Func<TSource, bool>>>();
        }

        internal override async Task SetEntities(Option<IQueryable<TSource>> allEntities, Option<IQueryable<TSource>> selectableEntities)
        {
            if (allEntities.TryGetValue(out var all))
            {
                _availableValues = Option.Some(await CreateSelectorList(all));
            }

            if (selectableEntities.TryGetValue(out var selectable))
            {
                _selectableValues = Option.Some(await CreateSelectorList(selectable));
            }
        }

        #endregion

        #region Private Methods

        [NotNull]
        private async Task<IReadOnlyList<TValue>> CreateSelectorList([NotNull] IQueryable<TSource> queryable)
        {
            return queryable is IAsyncEnumerable<TSource>
                ? await queryable.Select(Selector).Distinct().ToListAsync()
                : queryable.Select(Selector).Distinct().ToList();
        }

        [NotNull]
        private IReadOnlyList<Item<TValue>> CombineLists()
        {
            Dictionary<TValue, Item<TValue>> entities;
            if (_availableValues.TryGetValue(out var available))
            {
                entities = CreateDictionary(available, false);
                if (_selectableValues.TryGetValue(out var selectable))
                {
                    SetSelectableStatus(selectable, entities);
                }
            }
            else if (_selectableValues.TryGetValue(out var selectable))
            {
                entities = CreateDictionary(selectable, true);
            }
            else
            {
                return SelectedValues.Select(value => new Item<TValue> { Value = value, IsSelected = true }).ToList();
            }

            SetSelectedStatus(SelectedValues, entities);

            return entities.Values.ToList();
        }

        [NotNull]
        private static Dictionary<TValue, Item<TValue>> CreateDictionary([NotNull] IEnumerable<TValue> values, bool canBeSelected)
        {
            return values.ToDictionary(value => value, value => new Item<TValue> { Value = value, CanBeSelected = canBeSelected });
        }

        private static void SetSelectedStatus([NotNull] IEnumerable<TValue> selectedValues, IReadOnlyDictionary<TValue, Item<TValue>> dictionary)
        {
            foreach (var selectedValue in selectedValues)
            {
                if (dictionary.TryGetValue(selectedValue, out var selectable))
                {
                    selectable.IsSelected = true;
                }
            }
        }

        private static void SetSelectableStatus([NotNull] IEnumerable<TValue> selectableValues, [NotNull] Dictionary<TValue, Item<TValue>> dictionary)
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