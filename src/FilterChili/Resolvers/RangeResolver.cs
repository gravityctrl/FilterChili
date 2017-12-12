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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GravityCTRL.FilterChili.Resolvers
{
    public abstract class RangeResolver<TSource, TSelector> : DomainResolver<TSource, TSelector>
    {
        private bool _needsToBeResolved;

        internal override bool NeedsToBeResolved
        {
            get => _needsToBeResolved;
            set => _needsToBeResolved = value;
        }

        [UsedImplicitly]
        public Range<TSelector> TotalRange { get; private set; }

        [UsedImplicitly]
        public Range<TSelector> SelectableRange { get; private set; }

        [UsedImplicitly]
        public Range<TSelector> SelectedRange { get; }

        protected internal RangeResolver(string name, Expression<Func<TSource, TSelector>> selector, TSelector min, TSelector max) : base(name, selector)
        {
            _needsToBeResolved = true;
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
            try
            {
                var min = domainToken.Value<TSelector>("min");
                var max = domainToken.Value<TSelector>("max");
                Set(min, max);
            }
            catch (JsonSerializationException)
            {
                return false;
            }

            return true;
        }

        #region Internal Methods

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