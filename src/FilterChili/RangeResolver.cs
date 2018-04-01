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
using GravityCTRL.FilterChili.Resolvers;
using GravityCTRL.FilterChili.Resolvers.Interfaces;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace GravityCTRL.FilterChili
{
    public class RangeResolver<TSource, TValue> 
        : FilterResolver<RangeResolver<TSource, TValue>, TSource, TValue>, IRangeResolver<TValue>
            where TValue : IComparable
    {
        private bool _needsToBeResolved;

        internal override bool NeedsToBeResolved
        {
            get => _needsToBeResolved;
            set => _needsToBeResolved = value;
        }

        private readonly TValue _min;

        private readonly TValue _max;

        public override string FilterType { get; } = "Range";

        [UsedImplicitly]
        public Range<TValue> TotalRange { get; private set; }

        [UsedImplicitly]
        public Range<TValue> SelectableRange { get; private set; }

        [UsedImplicitly]
        public Range<TValue> SelectedRange { get; }

        internal RangeResolver([NotNull] Expression<Func<TSource, TValue>> selector, TValue min, TValue max) : base(selector)
        {
            _needsToBeResolved = true;
            _min = min;
            _max = max;
            SelectedRange = new Range<TValue>(min, max);
        }

        public void Set(TValue min, TValue max)
        {
            SelectedRange.Min = min;
            SelectedRange.Max = max;
            _needsToBeResolved = true;
        }

        public override bool TrySet([CanBeNull] JToken filterToken)
        {
            if (filterToken == null)
            {
                return false;
            }

            var minToken = filterToken.SelectToken("min");
            var maxToken = filterToken.SelectToken("max");
            if (minToken == null || maxToken == null)
            {
                return false;
            }

            var min = minToken.ToObject<TValue>();
            var max = maxToken.ToObject<TValue>();
            Set(min, max);
            return true;
        }

        #region Internal Methods

        protected override Expression<Func<TSource, bool>> FilterExpression()
        {
            if (_min.CompareTo(SelectedRange.Min) < 0 && _max.CompareTo(SelectedRange.Max) > 0)
            {
                var minConstant = Expression.Constant(SelectedRange.Min);
                var maxConstant = Expression.Constant(SelectedRange.Max);
                var greaterThanExpression = Expression.GreaterThanOrEqual(Selector.Body, minConstant);
                var lessThanExpression = Expression.LessThanOrEqual(Selector.Body, maxConstant);
                var andExpression = Expression.AndAlso(greaterThanExpression, lessThanExpression);
                return Expression.Lambda<Func<TSource, bool>>(andExpression, Selector.Parameters);
            }

            if (_max.CompareTo(SelectedRange.Max) > 0)
            {
                var maxConstant = Expression.Constant(SelectedRange.Max);
                var lessThanExpression = Expression.LessThanOrEqual(Selector.Body, maxConstant);
                return Expression.Lambda<Func<TSource, bool>>(lessThanExpression, Selector.Parameters);
            }

            // ReSharper disable once InvertIf
            if (_min.CompareTo(SelectedRange.Min) < 0)
            {
                var minConstant = Expression.Constant(SelectedRange.Min);
                var greaterThanExpression = Expression.GreaterThanOrEqual(Selector.Body, minConstant);
                return Expression.Lambda<Func<TSource, bool>>(greaterThanExpression, Selector.Parameters);
            }

            return null;
        }

        internal override async Task SetEntities(Option<IQueryable<TSource>> allEntities, Option<IQueryable<TSource>> selectableEntities)
        {
            if (allEntities.TryGetValue(out var all))
            {
                TotalRange = await SetRange(all.Select(Selector));
            }

            if (selectableEntities.TryGetValue(out var selectable))
            {
                SelectableRange = await SetRange(selectable.Select(Selector));
            }
        }

        [ItemCanBeNull]
        private static async Task<Range<TValue>> SetRange([NotNull] IQueryable<TValue> queryable)
        {
            if (queryable is IAsyncEnumerable<TValue> _)
            {
                return await ResolveRangeAsync(queryable);
            }

            return ResolveRange(queryable);
        }

        [CanBeNull]
        private static Range<TValue> ResolveRange([NotNull] IQueryable<TValue> queryable)
        {
            if (!queryable.Any())
            {
                return null;
            }

            var min = queryable.Min();
            var max = queryable.Max();
            return new Range<TValue>(min, max);
        }

        [ItemCanBeNull]
        private static async Task<Range<TValue>> ResolveRangeAsync([NotNull] IQueryable<TValue> queryable)
        {
            if (!await queryable.AnyAsync())
            {
                return null;
            }
            
            var min = await queryable.MinAsync();
            var max = await queryable.MaxAsync();
            return new Range<TValue>(min, max);
        }

        #endregion
    }
}