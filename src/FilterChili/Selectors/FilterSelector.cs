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
using GravityCTRL.FilterChili.Exceptions;
using GravityCTRL.FilterChili.Extensions;
using GravityCTRL.FilterChili.Models;
using GravityCTRL.FilterChili.Resolvers;
using GravityCTRL.FilterChili.Resolvers.Interfaces;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace GravityCTRL.FilterChili.Selectors
{
    public abstract class FilterSelector<TSource>
    {
        #region Internal Methods

        internal abstract CalculationStrategy CalculationStrategy { get; }

        internal abstract string Name { get; }

        internal abstract bool HasDomainResolver { get; }

        internal abstract bool NeedsToBeResolved { get; set; }

        internal abstract IQueryable<TSource> ApplyFilter(IQueryable<TSource> queryable);

        internal abstract Task SetEntities(Option<IQueryable<TSource>> allEntities, Option<IQueryable<TSource>> selectableEntities);

        internal abstract DomainResolver<TSource> Domain();

        internal abstract bool HasName(string name);

        internal abstract bool TrySet<TSelector>(TSelector value);

        internal abstract bool TrySet<TSelector>(TSelector min, TSelector max);

        internal abstract bool TrySet(JToken domainToken);

        internal FilterSelector() {}

        #endregion
    }

    public abstract class FilterSelector<TSource, TSelector> : FilterSelector<TSource> where TSelector : IComparable
    {
        internal override string Name => GetType().FormattedName();

        internal override CalculationStrategy CalculationStrategy => DomainResolver.CalculationStrategy;

        protected readonly Expression<Func<TSource, TSelector>> Selector;

        protected DomainResolver<TSource, TSelector> DomainResolver { private get; set; }

        internal override bool HasDomainResolver => DomainResolver != null;

        internal override bool NeedsToBeResolved
        {
            get => DomainResolver?.NeedsToBeResolved ?? false;
            set
            {
                if (DomainResolver == null)
                {
                    throw new MissingResolverException(Name);
                }

                DomainResolver.NeedsToBeResolved = value;
            }
        }

        internal FilterSelector(Expression<Func<TSource, TSelector>> selector)
        {
            Selector = selector;
        }

        #region Overridden Methods

        internal override IQueryable<TSource> ApplyFilter(IQueryable<TSource> queryable)
        {
            if (DomainResolver == null)
            {
                throw new MissingResolverException(Name);
            }

            return DomainResolver.ExecuteFilter(queryable);
        }

        internal override async Task SetEntities([NotNull] Option<IQueryable<TSource>> allEntities, [NotNull] Option<IQueryable<TSource>> selectableEntities)
        {
            if (DomainResolver == null)
            {
                throw new MissingResolverException(Name);
            }

            await DomainResolver.SetEntities(allEntities, selectableEntities);
        }

        [NotNull]
        internal override DomainResolver<TSource> Domain()
        {
            var resolver = DomainResolver ?? throw new MissingResolverException(Name);
            return resolver;
        }

        internal override bool HasName([NotNull] string name)
        {
            return DomainResolver?.Name == name;
        }

        internal override bool TrySet(JToken domainToken)
        {
            return DomainResolver?.TrySet(domainToken) ?? false;
        }

        internal override bool TrySet<TSelectorTarget>(TSelectorTarget value)
        {
            switch (value)
            {
                case TSelector valueTarget:
                {
                    return TrySet(valueTarget);
                }
                case IEnumerable<TSelector> targetValues:
                {
                    return TrySet(targetValues);
                }
                default:
                {
                    return false;
                }
            }
        }

        internal override bool TrySet<TSelectorTarget>(TSelectorTarget min, TSelectorTarget max)
        {
            if (min is TSelector minTarget && max is TSelector maxTarget)
            {
                return TrySet(minTarget, maxTarget);
            }

            return false;
        }

        #endregion

        #region Private Methods

        private bool TrySet(TSelector value)
        {
            switch (DomainResolver)
            {
                case IComparisonResolver<TSelector> target:
                {
                    target.Set(value);
                    return true;
                }
                default:
                {
                    return false;
                }
            }
        }

        private bool TrySet(TSelector min, TSelector max)
        {
            switch (DomainResolver)
            {
                case IRangeResolver<TSelector> target:
                {
                    target.Set(min, max);
                    return true;
                }
                default:
                {
                    return false;
                }
            }
        }

        private bool TrySet(IEnumerable<TSelector> values)
        {
            switch (DomainResolver)
            {
                case IListResolver<TSelector> listTarget:
                {
                    listTarget.Set(values);
                    return true;
                }
                case IGroupResolver<TSelector> groupTarget:
                {
                    groupTarget.Set(values);
                    return true;
                }
                default:
                {
                    return false;
                }
            }
        }

        #endregion
    }
}