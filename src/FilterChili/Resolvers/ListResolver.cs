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
using GravityCTRL.FilterChili.Models;
using GravityCTRL.FilterChili.Serialization;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GravityCTRL.FilterChili.Resolvers
{
    public abstract class ListResolver<TSource, TSelector> : DomainResolver<TSource, TSelector>
    {
        private bool _needsToBeResolved;

        internal override bool NeedsToBeResolved
        {
            get => _needsToBeResolved;
            set => _needsToBeResolved = value;
        }

        [NotNull]
        private IReadOnlyList<TSelector> _selectedValues;

        [CanBeNull]
        private IReadOnlyList<TSelector> _selectableValues;

        [CanBeNull]
        private IReadOnlyList<TSelector> _availableValues;

        [UsedImplicitly]
        public IReadOnlyList<Selectable<TSelector>> Values => CombineLists();

        protected internal ListResolver(string name, Expression<Func<TSource, TSelector>> selector) : base(name, selector)
        {
            _needsToBeResolved = true;
            _selectedValues = new List<TSelector>();
        }

        public void Set(IEnumerable<TSelector> selectedValues)
        {
            _selectedValues = selectedValues as IReadOnlyList<TSelector> ?? selectedValues.ToList();
            _selectableValues = null;
            _needsToBeResolved = true;
        }

        public void Set(params TSelector[] selectedValues)
        {
            _selectedValues = selectedValues as IReadOnlyList<TSelector> ?? selectedValues.ToList();
            _selectableValues = null;
            _needsToBeResolved = true;
        }

        public override bool TrySet(JToken domainToken)
        {
            try
            {
                var domain = domainToken.ToObject<Set<TSelector>>(JsonUtils.Serializer);
                Set(domain.Values);
            }
            catch (JsonSerializationException)
            {
                return false;
            }

            return true;
        }

        #region Internal Methods

        protected override async Task SetAvailableValues(IQueryable<TSelector> queryable)
        {
            _availableValues = queryable is IAsyncEnumerable<TSelector>
                ? await queryable.Distinct().ToListAsync()
                : queryable.Distinct().ToList();
        }

        protected override async Task SetSelectableValues(IQueryable<TSelector> queryable)
        {
            _selectableValues = queryable is IAsyncEnumerable<TSelector>
                ? await queryable.ToListAsync()
                : queryable.ToList();
        }

        protected override Expression<Func<IGrouping<TSelector, TSource>, bool>> FilterExpression()
        {
            if (_selectedValues.Any())
            {
                return group => _selectedValues.Contains(group.Key);
            }

            return null;
        }

        #endregion

        #region Private Methods

        private IReadOnlyList<Selectable<TSelector>> CombineLists()
        {
            if (_availableValues == null)
            {
                return _selectedValues.Select(value => new Selectable<TSelector> { Value = value }).ToList();
            }

            var entities = _availableValues.ToDictionary(value => value, value => new Selectable<TSelector> { Value = value });
            SetSelectedStatus(_selectedValues, entities);
            if (_selectableValues != null)
            {
                SetSelectableStatus(_selectableValues, entities);
            }

            return entities.Values.ToList();
        }

        private static void SetSelectedStatus(IEnumerable<TSelector> selectedValues, IReadOnlyDictionary<TSelector, Selectable<TSelector>> dictionary)
        {
            foreach (var selectedValue in selectedValues)
            {
                if (dictionary.TryGetValue(selectedValue, out var selectable))
                {
                    selectable.IsSelected = true;
                }
            }
        }

        private static void SetSelectableStatus(IEnumerable<TSelector> selectableValues, Dictionary<TSelector, Selectable<TSelector>> dictionary)
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