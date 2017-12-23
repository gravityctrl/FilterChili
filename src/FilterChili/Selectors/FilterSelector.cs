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
using System.Threading.Tasks;
using GravityCTRL.FilterChili.Providers;
using GravityCTRL.FilterChili.Resolvers;
using Newtonsoft.Json.Linq;

namespace GravityCTRL.FilterChili.Selectors
{
    public abstract class FilterSelector<TSource>
    {
        #region Internal Methods

        internal abstract bool NeedsToBeResolved { get; set; }

        internal abstract IQueryable<TSource> ApplyFilter(IQueryable<TSource> queryable);

        internal abstract Task SetAvailableEntities(IQueryable<TSource> queryable);

        internal abstract Task SetSelectableEntities(IQueryable<TSource> selectableItems);

        internal abstract DomainResolver<TSource> Domain();

        internal abstract bool HasName(string name);

        internal abstract bool TrySet<TSelector>(TSelector value);

        internal abstract bool TrySet<TSelector>(TSelector min, TSelector max);

        internal abstract bool TrySet<TSelector>(IEnumerable<TSelector> values);

        internal abstract bool TrySet(JToken domainToken);

        #endregion
    }

    public abstract class FilterSelector<TSource, TSelector, TDomainProvider> : FilterSelector<TSource> where TDomainProvider : DomainProvider<TSource, TSelector> where TSelector : IComparable
    {
        private readonly TDomainProvider _domainProvider;

        private DomainResolver<TSource, TSelector> _domainResolver;

        internal override bool NeedsToBeResolved
        {
            get => _domainResolver.NeedsToBeResolved;
            set => _domainResolver.NeedsToBeResolved = value;
        }

        protected internal FilterSelector(TDomainProvider domainProvider)
        {
            _domainProvider = domainProvider;
        }

        public TDomainResolver With<TDomainResolver>(Func<TDomainProvider, TDomainResolver> select) where TDomainResolver : DomainResolver<TSource, TSelector>
        {
            var resolver = select(_domainProvider);
            _domainResolver = resolver;
            return resolver;
        }

        #region Internal Methods

        internal override IQueryable<TSource> ApplyFilter(IQueryable<TSource> queryable)
        {
            return _domainResolver.ExecuteFilter(queryable);
        }

        internal override async Task SetAvailableEntities(IQueryable<TSource> queryable)
        {
            await _domainResolver.SetAvailableEntities(queryable);
        }

        internal override async Task SetSelectableEntities(IQueryable<TSource> selectableItems)
        {
            await _domainResolver.SetSelectableEntities(selectableItems);
        }

        internal override DomainResolver<TSource> Domain()
        {
            return _domainResolver;
        }

        internal override bool HasName(string name)
        {
            return _domainResolver.Name == name;
        }

        internal override bool TrySet(JToken domainToken)
        {
            return _domainResolver.TrySet(domainToken);
        }

        internal override bool TrySet<TSelectorTarget>(TSelectorTarget value)
        {
            if (value is TSelector valueTarget)
            {
                return TrySet(valueTarget);
            }

            return false;
        }

        internal override bool TrySet<TSelectorTarget>(TSelectorTarget min, TSelectorTarget max)
        {
            if (min is TSelector minTarget && max is TSelector maxTarget)
            {
                return TrySet(minTarget, maxTarget);
            }

            return false;
        }

        internal override bool TrySet<TSelectorTarget>(IEnumerable<TSelectorTarget> values)
        {
            if (values is IEnumerable<TSelector> targetValues)
            {
                return TrySet(targetValues);
            }

            return false;
        }

        #endregion

        #region Private Methods

        private bool TrySet(TSelector value)
        {
            // ReSharper disable once InvertIf
            if (_domainResolver is ComparisonResolver<TSource, TSelector> target)
            {
                target.Set(value);
                return true;
            }

            return false;
        }

        private bool TrySet(TSelector min, TSelector max)
        {
            // ReSharper disable once InvertIf
            if (_domainResolver is RangeResolver<TSource, TSelector> target)
            {
                target.Set(min, max);
                return true;
            }

            return false;
        }

        private bool TrySet(IEnumerable<TSelector> values)
        {
            // ReSharper disable once InvertIf
            if (_domainResolver is ListResolver<TSource, TSelector> target)
            {
                target.Set(values);
                return true;
            }

            return false;
        }

        #endregion
    }
}