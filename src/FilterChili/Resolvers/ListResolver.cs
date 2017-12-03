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
using GravityCTRL.FilterChili.Models;
using Microsoft.EntityFrameworkCore;

namespace GravityCTRL.FilterChili.Resolvers
{
    public abstract class ListResolver<TSource, TSelector> : DomainResolver<TSource, TSelector>
    {
        private bool _needsToBeResolved;

        internal override bool NeedsToBeResolved => _needsToBeResolved;

        private IReadOnlyList<TSelector> _selectedValues;
        private IReadOnlyList<TSelector> _selectableValues;
        private IReadOnlyList<TSelector> _allValues;

        public IReadOnlyList<Selectable<TSelector>> Values => CombineLists();

        protected internal ListResolver(string name, Expression<Func<TSource, TSelector>> selector) : base(name, selector)
        {
            _needsToBeResolved = true;
            _selectedValues = new List<TSelector>();
            _selectableValues = new List<TSelector>();
            _allValues = new List<TSelector>();
        }

        public void Set(IEnumerable<TSelector> selectedValues)
        {
            _selectedValues = selectedValues as IReadOnlyList<TSelector> ?? selectedValues.ToList();
            _needsToBeResolved = true;
        }

        public void Set(params TSelector[] selectedValues)
        {
            _selectedValues = selectedValues as IReadOnlyList<TSelector> ?? selectedValues.ToList();
            _needsToBeResolved = true;
        }

        #region Internal Methods

        internal override async Task Resolve(IQueryable<TSelector> allItems, IQueryable<TSelector> selectableItems)
        {
            _allValues = allItems is IAsyncEnumerable<TSelector>
                ? await allItems.Distinct().ToListAsync()
                : allItems.Distinct().ToList();

            _selectableValues = selectableItems is IAsyncEnumerable<TSelector>
                ? await selectableItems.ToListAsync()
                : selectableItems.ToList();

            _needsToBeResolved = false;
        }

        internal override Expression<Func<IGrouping<TSelector, TSource>, bool>> FilterExpression()
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
            var entities = _allValues.ToDictionary(value => value, value => new Selectable<TSelector> { Value = value });
            foreach (var selectedValue in _selectedValues)
            {
                if (entities.TryGetValue(selectedValue, out var selectable))
                {
                    selectable.IsSelected = true;
                }
            }

            foreach (var selectableValue in _selectableValues)
            {
                if (entities.TryGetValue(selectableValue, out var selectable))
                {
                    selectable.CanBeSelected = true;
                }
            }

            return entities.Values.ToList();
        }

        #endregion
    }

    public class StringListResolver<TSource> : ListResolver<TSource, string>
    {
        internal StringListResolver(string name, Expression<Func<TSource, string>> selector) : base(name, selector) { }
    }
}