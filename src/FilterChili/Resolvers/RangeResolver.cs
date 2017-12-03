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
    public abstract class RangeResolver<TSource, TSelector> : DomainResolver<TSource, TSelector>
    {
        private bool _needsToBeResolved;

        internal override bool NeedsToBeResolved => _needsToBeResolved;

        [UsedImplicitly]
        public Range<TSelector> TotalRange { get; }

        [UsedImplicitly]
        public Range<TSelector> SelectableRange { get; }

        [UsedImplicitly]
        public Range<TSelector> SelectedRange { get; }

        protected internal RangeResolver(string name, Expression<Func<TSource, TSelector>> selector, TSelector min, TSelector max) : base(name, selector)
        {
            _needsToBeResolved = true;
            SelectableRange = new Range<TSelector>(min, max);
            SelectedRange = new Range<TSelector>(min, max);
            TotalRange = new Range<TSelector>(min, max);
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
                var domain = domainToken.ToObject<Range<TSelector>>(JsonUtils.Serializer);
                Set(domain.Min, domain.Max);
            }
            catch (JsonSerializationException)
            {
                return false;
            }

            return true;
        }

        #region Internal Methods

        protected override async Task Resolve(IQueryable<TSelector> allItems, IQueryable<TSelector> selectableItems)
        {
            if (allItems is IAsyncEnumerable<TSelector>)
            {
                TotalRange.Min = await allItems.MinAsync();
                TotalRange.Max = await allItems.MaxAsync();
            }
            else
            {
                TotalRange.Min = allItems.Min();
                TotalRange.Max = allItems.Max();
            }

            if (selectableItems is IAsyncEnumerable<TSelector>)
            {
                SelectableRange.Min = await selectableItems.MinAsync();
                SelectableRange.Max = await selectableItems.MaxAsync();
            }
            else
            {
                SelectableRange.Min = selectableItems.Min();
                SelectableRange.Max = selectableItems.Max();
            }

            _needsToBeResolved = false;
        }

        #endregion
    }
}