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
using System.Reflection;
using System.Threading.Tasks;
using GravityCTRL.FilterChili.Expressions;
using GravityCTRL.FilterChili.Models;
using GravityCTRL.FilterChili.Resolvers;
using GravityCTRL.FilterChili.Resolvers.Interfaces;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace GravityCTRL.FilterChili
{
    public abstract class GroupResolver<TSource, TSelector, TGroupSelector> 
        : DomainResolver<GroupResolver<TSource, TSelector, TGroupSelector>, TSource, TSelector>, IGroupResolver<TSelector> 
            where TSelector : IComparable 
            where TGroupSelector : IComparable
    {
        // ReSharper disable StaticMemberInGenericType
        private static readonly PropertyInfo GroupIdentifierProperty;
        private static readonly PropertyInfo ValueProperty;
        private static readonly NewExpression NewKeyValuePairExpression;
        private static readonly ParameterExpression ParameterExpression;
        // ReSharper restore StaticMemberInGenericType

        private readonly Expression<Func<TSource, KeyValuePair>> _selectKeyValuePairExpression;

        private bool _needsToBeResolved;
        private IReadOnlyList<KeyValuePair> _availableValues;
        private IReadOnlyList<TSelector> _selectableValues;
        private IReadOnlyList<TSelector> _selectedValues;

        public override string FilterType { get; } = "Group";

        [UsedImplicitly]
        public IReadOnlyList<Group<TGroupSelector, TSelector>> Groups => CombineLists();

        internal override bool NeedsToBeResolved
        {
            get => _needsToBeResolved;
            set => _needsToBeResolved = value;
        }

        #region Constructors

        static GroupResolver()
        {
            var keyValuePairType = typeof(KeyValuePair);
            GroupIdentifierProperty = keyValuePairType.GetProperty(nameof(KeyValuePair.GroupIdentifier));
            ValueProperty = keyValuePairType.GetProperty(nameof(KeyValuePair.Value));
            NewKeyValuePairExpression = Expression.New(keyValuePairType);
            ParameterExpression = Expression.Parameter(GenericSourceType);
        }

        protected GroupResolver(Expression<Func<TSource, TSelector>> selector, Expression<Func<TSource, TGroupSelector>> groupSelector) : base(selector)
        {
            _selectedValues = new List<TSelector>();
            _needsToBeResolved = true;

            var memberBindings = new List<MemberBinding>
            {
                Expression.Bind(GroupIdentifierProperty, groupSelector.Body),
                Expression.Bind(ValueProperty, Selector.Body)
            };

            var memberInitExpression = Expression.MemberInit(NewKeyValuePairExpression, memberBindings);
            var rewrittenExpression = PredicateRewriter.Rewrite(ParameterExpression, memberInitExpression);
            _selectKeyValuePairExpression = Expression.Lambda<Func<TSource, KeyValuePair>>(rewrittenExpression, ParameterExpression);
        }

        #endregion

        #region Public Methods

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

        public void SetGroups(IEnumerable<TGroupSelector> selectedValues)
        {
            _selectedValues = _availableValues.Where(group => selectedValues.Contains(group.GroupIdentifier)).Select(group => group.Value).ToList();
            _selectableValues = null;
            _needsToBeResolved = true;
        }

        public void SetGroups(params TGroupSelector[] selectedValues)
        {
            _selectedValues = _availableValues.Where(group => selectedValues.Contains(group.GroupIdentifier)).Select(group => group.Value).ToList();
            _selectableValues = null;
            _needsToBeResolved = true;
        }

        #endregion

        #region Public Overrides

        public override bool TrySet(JToken domainToken)
        {
            var valuesToken = domainToken.SelectToken("values");
            if (valuesToken != null)
            {
                var values = valuesToken.Values<TSelector>();
                Set(values);
                return true;
            }

            var groupsToken = domainToken.SelectToken("groups");
            // ReSharper disable once InvertIf
            if (groupsToken != null)
            {
                var groups = groupsToken.Values<TGroupSelector>();
                SetGroups(groups);
                return true;
            }

            return false;
        }

        #endregion

        protected override Expression<Func<TSource, bool>> FilterExpression()
        {
            if (!_selectedValues.Any())
            {
                return null;
            }

            var selectedValueExpressions = _selectedValues.Select(selector => Expression.Constant(selector));
            var equalsExpressions = selectedValueExpressions.Select(expression => Expression.Equal(expression, Selector.Body));
            var orExpression = equalsExpressions.Or();
            return orExpression == null ? null : Expression.Lambda<Func<TSource, bool>>(orExpression, Selector.Parameters);
        }

        internal override async Task SetAvailableEntities(IQueryable<TSource> queryable)
        {
            var groupQueryable = queryable.Select(_selectKeyValuePairExpression);
            _availableValues = groupQueryable is IAsyncEnumerable<KeyValuePair>
                ? await groupQueryable.Distinct().ToListAsync()
                : groupQueryable.Distinct().ToList();
        }

        internal override async Task SetSelectableEntities(IQueryable<TSource> queryable)
        {
            _selectableValues = queryable is IAsyncEnumerable<TSource>
                ? await queryable.Select(Selector).Distinct().ToListAsync()
                : queryable.Select(Selector).Distinct().ToList();
        }

        #region Private Methods

        private IReadOnlyList<Group<TGroupSelector, TSelector>> CombineLists()
        {
            var groupDictionary = CreateGroupDictionary();
            SetSelectedStatus(_selectedValues, groupDictionary);
            if (_selectableValues != null)
            {
                SetSelectableStatus(_selectableValues, groupDictionary);
            }

            return groupDictionary.Select(kv =>
                new Group<TGroupSelector, TSelector>
                {
                    Identifier = kv.Key,
                    Values = kv.Value.Values.ToList()
                }).ToList();
        }

        private IReadOnlyDictionary<TGroupSelector, Dictionary<TSelector, Item<TSelector>>> CreateGroupDictionary()
        {
            var dictionary = new Dictionary<TGroupSelector, Dictionary<TSelector, Item<TSelector>>>();
            foreach (var availableValue in _availableValues)
            {
                var key = availableValue.GroupIdentifier;
                var value = availableValue.Value;

                var item = new Item<TSelector> { Value = value };
                if (!dictionary.ContainsKey(availableValue.GroupIdentifier))
                {
                    var valueDictionary = new Dictionary<TSelector, Item<TSelector>> { {value, item} };
                    dictionary.Add(key, valueDictionary);
                }
                else
                {
                    if (dictionary[key].ContainsKey(value))
                    {
                        continue;
                    }

                    dictionary[key].Add(value, item);
                }
            }

            return dictionary;
        }

        private static void SetSelectedStatus(IReadOnlyList<TSelector> selectedValues, IReadOnlyDictionary<TGroupSelector, Dictionary<TSelector, Item<TSelector>>> dictionary)
        {
            foreach (var valueDictionary in dictionary.Values)
            {
                foreach (var selectedValue in selectedValues)
                {
                    if (valueDictionary.TryGetValue(selectedValue, out var selectable))
                    {
                        selectable.IsSelected = true;
                    }
                }
            }
        }

        private static void SetSelectableStatus(IReadOnlyList<TSelector> selectableValues, IReadOnlyDictionary<TGroupSelector, Dictionary<TSelector, Item<TSelector>>> dictionary)
        {
            foreach (var valueDictionary in dictionary.Values)
            {
                foreach (var selectable in valueDictionary.Values)
                {
                    selectable.CanBeSelected = false;
                }

                foreach (var selectableValue in selectableValues)
                {
                    if (valueDictionary.TryGetValue(selectableValue, out var selectable))
                    {
                        selectable.CanBeSelected = true;
                    }
                }
            }
        }

        #endregion

        #region Private Classes

        private class KeyValuePair
        {
            public TGroupSelector GroupIdentifier { get; [UsedImplicitly] set; }
            public TSelector Value { get; [UsedImplicitly] set; }
        }

        #endregion
    }
}
