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
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GravityCTRL.FilterChili.Providers;
using GravityCTRL.FilterChili.Resolvers;

namespace GravityCTRL.FilterChili
{
    public abstract class FilterSelector<TSource>
    {
        #region Internal Methods

        internal abstract IQueryable<TSource> ApplyFilter(IQueryable<TSource> queryable);

        internal abstract Task Resolve(IQueryable<TSource> queryable, IQueryable<TSource> selectableItems);

        internal abstract DomainResolver<TSource> Domain();

        #endregion
    }

    public abstract class FilterSelector<TSource, TSelector, TDomainProvider> : FilterSelector<TSource> where TDomainProvider : DomainProvider<TSource, TSelector>
    {
        private readonly TDomainProvider _domainProvider;

        private DomainResolver<TSource, TSelector> _domainResolver;

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

        internal override async Task Resolve(IQueryable<TSource> queryable, IQueryable<TSource> selectableItems)
        {
            await _domainResolver.Resolve(queryable, selectableItems);
        }

        internal override DomainResolver<TSource> Domain()
        {
            return _domainResolver;
        }

        #endregion
    }

    public class StringFilterSelector<TSource> : FilterSelector<TSource, string, StringDomainProvider<TSource>>
    {
        internal StringFilterSelector(Action onChange, Expression<Func<TSource, string>> valueSelector) : base(new StringDomainProvider<TSource>(onChange, valueSelector)) { }
    }

    public class IntFilterSelector<TSource> : FilterSelector<TSource, int, IntDomainProvider<TSource>>
    {
        internal IntFilterSelector(Action onChange, Expression<Func<TSource, int>> valueSelector) : base(new IntDomainProvider<TSource>(onChange, valueSelector)) { }
    }
}