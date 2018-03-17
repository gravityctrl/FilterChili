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
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace GravityCTRL.FilterChili.Resolvers
{
    public abstract class RangeResolver<TSource, TSelector> : DomainResolver<RangeResolver<TSource, TSelector>, TSource, TSelector> where TSelector : IComparable
    {
        private bool _needsToBeResolved;

        internal override bool NeedsToBeResolved
        {
            get => _needsToBeResolved;
            set => _needsToBeResolved = value;
        }

        private readonly TSelector _min;

        private readonly TSelector _max;

        public override string FilterType { get; } = "Range";

        [UsedImplicitly]
        public Range<TSelector> TotalRange { get; private set; }

        [UsedImplicitly]
        public Range<TSelector> SelectableRange { get; private set; }

        [UsedImplicitly]
        public Range<TSelector> SelectedRange { get; }

        protected internal RangeResolver(Expression<Func<TSource, TSelector>> selector, TSelector min, TSelector max) : base(selector)
        {
            _needsToBeResolved = true;
            _min = min;
            _max = max;
            SelectedRange = new Range<TSelector>(min, max);
        }

        public void Set(TSelector min, TSelector max)
        {
            SelectedRange.Min = min;
            SelectedRange.Max = max;
            _needsToBeResolved = true;
        }

        public override bool TrySet(JToken domainToken)
        {
            var minToken = domainToken.SelectToken("min");
            var maxToken = domainToken.SelectToken("max");
            if (minToken == null || maxToken == null)
            {
                return false;
            }

            var min = minToken.ToObject<TSelector>();
            var max = maxToken.ToObject<TSelector>();
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
                var andExpression = Expression.And(greaterThanExpression, lessThanExpression);
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

        protected override async Task SetAvailableValues(IQueryable<TSelector> allValues)
        {
            TotalRange = await SetRange(allValues);
        }

        protected override async Task SetSelectableValues(IQueryable<TSelector> selectableItems)
        {
            SelectableRange = await SetRange(selectableItems);
        }

        private static async Task<Range<TSelector>> SetRange(IQueryable<TSelector> queryable)
        {
            if (queryable is IAsyncEnumerable<TSelector> _)
            {
                return await ResolveRangeAsync(queryable);
            }

            return ResolveRange(queryable);
        }

        private static Range<TSelector> ResolveRange(IQueryable<TSelector> queryable)
        {
            if (!queryable.Any())
            {
                return null;
            }

            var min = queryable.Min();
            var max = queryable.Max();
            return new Range<TSelector>(min, max);
        }

        private static async Task<Range<TSelector>> ResolveRangeAsync(IQueryable<TSelector> queryable)
        {
            if (!await queryable.AnyAsync())
            {
                return null;
            }
            
            var min = await queryable.MinAsync();
            var max = await queryable.MaxAsync();
            return new Range<TSelector>(min, max);
        }

        #endregion
    }
}