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
using GravityCTRL.FilterChili.Extensions;
using GravityCTRL.FilterChili.Models;
using GravityCTRL.FilterChili.Resolvers;
using GravityCTRL.FilterChili.Resolvers.Interfaces;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace GravityCTRL.FilterChili
{
    public class GroupResolver<TSource, TSelector, TGroupSelector> 
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
        private Option<TGroupSelector> _defaultGroupIdentifier;

        internal IReadOnlyList<TSelector> SelectedValues;

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

        internal GroupResolver([NotNull] Expression<Func<TSource, TSelector>> selector, [NotNull] Expression<Func<TSource, TGroupSelector>> groupSelector) : base(selector)
        {
            SelectedValues = new List<TSelector>();

            _defaultGroupIdentifier = Option.None<TGroupSelector>();
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

        [UsedImplicitly]
        public void Set(IEnumerable<TSelector> selectedValues)
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

        [UsedImplicitly]
        public void SetGroups(IEnumerable<TGroupSelector> selectedValues)
        {
            if (_availableValues == null)
            {
                return;
            }

            SelectedValues = _availableValues.Where(group => selectedValues.Contains(group.GroupIdentifier)).Select(group => group.Value).ToList();
            _selectableValues = null;
            _needsToBeResolved = true;
        }

        [UsedImplicitly]
        public void SetGroups(params TGroupSelector[] selectedValues)
        {
            if (_availableValues == null)
            {
                return;
            }

            SelectedValues = _availableValues.Where(group => selectedValues.Contains(group.GroupIdentifier)).Select(group => group.Value).ToList();
            _selectableValues = null;
            _needsToBeResolved = true;
        }

        [NotNull]
        [UsedImplicitly]
        public GroupResolver<TSource, TSelector, TGroupSelector> UseDefaultGroup(TGroupSelector defaultGroupIdentifier)
        {
            _defaultGroupIdentifier = Option.Some(defaultGroupIdentifier);
            return this;
        }

        #endregion

        #region Public Overrides

        [UsedImplicitly]
        public override bool TrySet([CanBeNull] JToken domainToken)
        {
            if (domainToken == null)
            {
                return false;
            }

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

        #endregion

        #region Private Methods

        private IReadOnlyList<Group<TGroupSelector, TSelector>> CombineLists()
        {
            if (_availableValues == null)
            {
                var list = new List<Group<TGroupSelector, TSelector>>();

                if (SelectedValues.Count <= 0 || !_defaultGroupIdentifier.TryGetValue(out var identifier))
                {
                    return list;
                }

                var group = new Group<TGroupSelector, TSelector>
                {
                    Identifier = identifier,
                    Values = SelectedValues.Select(value => new Item<TSelector> { Value = value, IsSelected = true }).ToList()
                };

                list.Add(group);

                return list;
            }

            var groupDictionary = CreateGroupDictionary();
            SetSelectedStatus(SelectedValues, groupDictionary);
            if (_selectableValues != null)
            {
                SetSelectableStatus(_selectableValues, groupDictionary);
            }

            return groupDictionary.Select(kv => new Group<TGroupSelector, TSelector>
            {
                Identifier = kv.Key,
                Values = kv.Value.Values.ToList()
            }).ToList();
        }

        [NotNull]
        private IReadOnlyDictionary<TGroupSelector, Dictionary<TSelector, Item<TSelector>>> CreateGroupDictionary()
        {
            var useDefaultIdentifier = _defaultGroupIdentifier.TryGetValue(out var identifier);
            var dictionary = new Dictionary<TGroupSelector, Dictionary<TSelector, Item<TSelector>>>();

            foreach (var availableValue in _availableValues)
            {
                var key = availableValue.GroupIdentifier;
                if (key == null)
                {
                    if (useDefaultIdentifier)
                    {
                        key = identifier;
                    }
                    else
                    {
                        continue;
                    }
                }

                var value = availableValue.Value;
                var item = new Item<TSelector> { Value = value };

                if (!dictionary.ContainsKey(key))
                {
                    var valueDictionary = new Dictionary<TSelector, Item<TSelector>> { { value, item } };
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

        private static void SetSelectedStatus(IReadOnlyList<TSelector> selectedValues, [NotNull] IReadOnlyDictionary<TGroupSelector, Dictionary<TSelector, Item<TSelector>>> dictionary)
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

        private static void SetSelectableStatus(IReadOnlyList<TSelector> selectableValues, [NotNull] IReadOnlyDictionary<TGroupSelector, Dictionary<TSelector, Item<TSelector>>> dictionary)
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

        private sealed class KeyValuePair
        {
            public TGroupSelector GroupIdentifier { get; [UsedImplicitly] set; }
            public TSelector Value { get; [UsedImplicitly] set; }
        }

        #endregion
    }
}
