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
    public sealed class GroupResolver<TSource, TValue, TGroupIdentifier> 
        : FilterResolver<GroupResolver<TSource, TValue, TGroupIdentifier>, TSource, TValue>, IGroupResolver<TValue> 
            where TValue : IComparable 
            where TGroupIdentifier : IComparable
    {
        [NotNull] private readonly Expression<Func<TSource, TGroupIdentifier>> _groupSelector;

        // ReSharper disable StaticMemberInGenericType
        private static readonly PropertyInfo GroupIdentifierProperty;
        private static readonly PropertyInfo ValueProperty;
        private static readonly NewExpression NewKeyValuePairExpression;
        private static readonly ParameterExpression ParameterExpression;
        // ReSharper restore StaticMemberInGenericType

        private readonly Expression<Func<TSource, KeyValuePair>> _selectKeyValuePairExpression;

        private Option<TGroupIdentifier> _defaultGroupIdentifier;
        private Option<IReadOnlyList<KeyValuePair>> _groupList;
        private Option<IReadOnlyList<TValue>> _selectableList;

        internal override bool NeedsToBeResolved { get; set; }

        public override string FilterType { get; } = "Group";

        [NotNull]
        internal Either<List<TValue>, List<TGroupIdentifier>> Selection { get; private set; }

        [NotNull]
        [UsedImplicitly]
        public IReadOnlyList<Group<TGroupIdentifier, TValue>> Groups => CombineLists();

        #region Constructors

        static GroupResolver()
        {
            var keyValuePairType = typeof(KeyValuePair);
            GroupIdentifierProperty = keyValuePairType.GetProperty(nameof(KeyValuePair.GroupIdentifier));
            ValueProperty = keyValuePairType.GetProperty(nameof(KeyValuePair.Value));
            NewKeyValuePairExpression = Expression.New(keyValuePairType);
            ParameterExpression = Expression.Parameter(GenericSourceType);
        }

        internal GroupResolver([NotNull] Expression<Func<TSource, TValue>> selector, [NotNull] Expression<Func<TSource, TGroupIdentifier>> groupSelector) : base(selector)
        {
            _groupSelector = groupSelector;
            NeedsToBeResolved = true;
            Selection = new List<TValue>();

            _defaultGroupIdentifier = Option.None<TGroupIdentifier>();

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
        public void Set([NotNull] IEnumerable<TValue> selectedValues)
        {
            Selection = selectedValues as List<TValue> ?? selectedValues.ToList();
            NeedsToBeResolved = true;
        }

        [UsedImplicitly]
        public void Set([NotNull] params TValue[] selectedValues)
        {
            Selection = selectedValues as List<TValue> ?? selectedValues.ToList();
            NeedsToBeResolved = true;
        }

        [UsedImplicitly]
        public void SetGroups([NotNull] IEnumerable<TGroupIdentifier> selectedValues)
        {
            Selection = selectedValues as List<TGroupIdentifier> ?? selectedValues.ToList();
            NeedsToBeResolved = true;
        }

        [UsedImplicitly]
        public void SetGroups([NotNull] params TGroupIdentifier[] selectedValues)
        {
            Selection = selectedValues as List<TGroupIdentifier> ?? selectedValues.ToList();
            NeedsToBeResolved = true;
        }

        [NotNull]
        [UsedImplicitly]
        public GroupResolver<TSource, TValue, TGroupIdentifier> UseDefaultGroup(TGroupIdentifier defaultGroupIdentifier)
        {
            _defaultGroupIdentifier = Option.Some(defaultGroupIdentifier);
            return this;
        }

        #endregion

        #region Public Overrides

        [UsedImplicitly]
        public override bool TrySet([CanBeNull] JToken filterToken)
        {
            if (filterToken == null)
            {
                return false;
            }

            var valuesToken = filterToken.SelectToken("values");
            if (valuesToken != null)
            {
                var values = valuesToken.Values<TValue>();
                Set(values);
                return true;
            }

            var groupsToken = filterToken.SelectToken("groups");
            // ReSharper disable once InvertIf
            if (groupsToken != null)
            {
                var groups = groupsToken.Values<TGroupIdentifier>();
                SetGroups(groups);
                return true;
            }

            return false;
        }

        #endregion

        #region Internal Methods

        internal override Option<Expression<Func<TSource, bool>>> FilterExpression()
        {
            return Selection.Match
            (
                FilterExpressionForValues,
                FilterExpressionForGroups
            );
        }

        internal override async Task SetEntities(Option<IQueryable<TSource>> allEntities, Option<IQueryable<TSource>> selectableEntities)
        {
            var hasProvidedAllEntities = allEntities.TryGetValue(out var all);
            var hasProvidedSelectableEntities = selectableEntities.TryGetValue(out var selectable);

            if (hasProvidedAllEntities)
            {
                var groupQueryable = all.Select(_selectKeyValuePairExpression);
                _groupList = Option.Some(await CreateGroupList(groupQueryable));
                _selectableList = hasProvidedSelectableEntities 
                    ? Option.Some(await CreateSelectorList(selectable)) 
                    : Option.None<IReadOnlyList<TValue>>();
            }
            else if (hasProvidedSelectableEntities)
            {
                var selectableGroupQueryable = selectable.Select(_selectKeyValuePairExpression);
                _groupList = Option.Some(await CreateGroupList(selectableGroupQueryable));
                _selectableList = Option.None<IReadOnlyList<TValue>>();
            }
        }

        #endregion

        #region Private Methods

        [NotNull]
        private Option<Expression<Func<TSource, bool>>> FilterExpressionForValues([NotNull] IReadOnlyList<TValue> values)
        {
            if (!values.Any())
            {
                return Option.None<Expression<Func<TSource, bool>>>();
            }

            var selectedValueExpressions = values.Select(selector => Expression.Constant(selector));
            var equalsExpressions = selectedValueExpressions.Select(expression => Expression.Equal(expression, Selector.Body));
            var orExpression = equalsExpressions.Or();
            return orExpression.TryGetValue(out var value)
                ? Option.Some(Expression.Lambda<Func<TSource, bool>>(value, Selector.Parameters))
                : Option.None<Expression<Func<TSource, bool>>>();
        }

        [NotNull]
        private Option<Expression<Func<TSource, bool>>> FilterExpressionForGroups([NotNull] IReadOnlyList<TGroupIdentifier> groups)
        {
            if (!groups.Any())
            {
                return Option.None<Expression<Func<TSource, bool>>>();
            }

            var selectedValueExpressions = groups.Select(selector => Expression.Constant(selector));
            var equalsExpressions = selectedValueExpressions.Select(expression => Expression.Equal(expression, _groupSelector.Body));
            var orExpression = equalsExpressions.Or();
            return orExpression.TryGetValue(out var value)
                ? Option.Some(Expression.Lambda<Func<TSource, bool>>(value, _groupSelector.Parameters))
                : Option.None<Expression<Func<TSource, bool>>>();
        }

        [NotNull]
        private static async Task<IReadOnlyList<KeyValuePair>> CreateGroupList([NotNull] IQueryable<KeyValuePair> groupQueryable)
        {
            return groupQueryable is IAsyncEnumerable<KeyValuePair>
                ? await groupQueryable.Distinct().ToListAsync()
                : groupQueryable.Distinct().ToList();
        }

        [NotNull]
        private async Task<IReadOnlyList<TValue>> CreateSelectorList([NotNull] IQueryable<TSource> queryable)
        {
            return queryable is IAsyncEnumerable<TSource>
                ? await queryable.Select(Selector).Distinct().ToListAsync()
                : queryable.Select(Selector).Distinct().ToList();
        }

        [NotNull]
        private IReadOnlyList<Group<TGroupIdentifier, TValue>> CombineLists()
        {
            if (!_groupList.TryGetValue(out var source))
            {
                return CreateResultUsingSelectedValues();
            }

            var groupDictionary = CreateGroupDictionary(source, false);
            if (_selectableList.TryGetValue(out var selectable))
            {
                SetSelectableStatus(selectable, groupDictionary);
            }

            SetSelectedStatus(Selection, groupDictionary);

            var result = groupDictionary.Select(kv => new Group<TGroupIdentifier, TValue>
            {
                Identifier = kv.Key,
                Values = kv.Value.Values.ToList()
            });

            return result.ToList();
        }

        [NotNull]
        private IReadOnlyDictionary<TGroupIdentifier, Dictionary<TValue, Item<TValue>>> CreateGroupDictionary([NotNull] IEnumerable<KeyValuePair> source, bool canBeSelected)
        {
            var useDefaultIdentifier = _defaultGroupIdentifier.TryGetValue(out var identifier);
            var dictionary = new Dictionary<TGroupIdentifier, Dictionary<TValue, Item<TValue>>>();

            foreach (var keyValuePair in source)
            {
                var key = keyValuePair.GroupIdentifier;
                
                // ReSharper disable once CompareNonConstrainedGenericWithNull
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

                var value = keyValuePair.Value;
                var item = new Item<TValue> { Value = value, CanBeSelected = canBeSelected };

                if (!dictionary.ContainsKey(key))
                {
                    var valueDictionary = new Dictionary<TValue, Item<TValue>> { { value, item } };
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

        [NotNull]
        private IReadOnlyList<Group<TGroupIdentifier, TValue>> CreateResultUsingSelectedValues()
        {
            return Selection.Match
            (
                CreateResultUsingSelectedValues,
                CreateResultUsingSelectedGroups
            );
        }

        [NotNull]
        private List<Group<TGroupIdentifier, TValue>> CreateResultUsingSelectedValues([NotNull] IReadOnlyList<TValue> values)
        {
            if (values.Count <= 0 || !_defaultGroupIdentifier.TryGetValue(out var identifier))
            {
                return new List<Group<TGroupIdentifier, TValue>>();
            }

            var group = new Group<TGroupIdentifier, TValue>
            {
                Identifier = identifier,
                Values = values.Select(value => new Item<TValue> { Value = value, IsSelected = true, CanBeSelected = false }).ToList()
            };

            return new List<Group<TGroupIdentifier, TValue>> { group };
        }

        [NotNull]
        private static List<Group<TGroupIdentifier, TValue>> CreateResultUsingSelectedGroups([NotNull] IReadOnlyList<TGroupIdentifier> groups)
        {
            var result = groups.Select(group => new Group<TGroupIdentifier, TValue> { Identifier = group });
            return result.ToList();
        }

        private static void SetSelectedStatus(Either<List<TValue>, List<TGroupIdentifier>> selection, [NotNull] IReadOnlyDictionary<TGroupIdentifier, Dictionary<TValue, Item<TValue>>> dictionary)
        {
            selection.Match
            (
                values => SetSelectedStatus(dictionary, values),
                groups => SetSelectedStatus(dictionary, groups)
            );
        }

        private static void SetSelectedStatus([NotNull] IReadOnlyDictionary<TGroupIdentifier, Dictionary<TValue, Item<TValue>>> dictionary, [NotNull] IEnumerable<TGroupIdentifier> groups)
        {
            foreach (var selectedGroup in groups)
            {
                if (!dictionary.TryGetValue(selectedGroup, out var dict))
                {
                    continue;
                }

                foreach (var selectedValue in dict.Values)
                {
                    selectedValue.IsSelected = true;
                }
            }
        }

        private static void SetSelectedStatus([NotNull] IReadOnlyDictionary<TGroupIdentifier, Dictionary<TValue, Item<TValue>>> dictionary, [NotNull] IReadOnlyList<TValue> values)
        {
            foreach (var valueDictionary in dictionary.Values)
            {
                foreach (var selectedValue in values)
                {
                    if (valueDictionary.TryGetValue(selectedValue, out var selectable))
                    {
                        selectable.IsSelected = true;
                    }
                }
            }
        }

        private static void SetSelectableStatus(IReadOnlyList<TValue> selectableValues, [NotNull] IReadOnlyDictionary<TGroupIdentifier, Dictionary<TValue, Item<TValue>>> dictionary)
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
            public TGroupIdentifier GroupIdentifier { get; [UsedImplicitly] set; }
            public TValue Value { get; [UsedImplicitly] set; }
        }

        #endregion
    }
}
