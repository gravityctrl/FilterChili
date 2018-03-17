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
using GravityCTRL.FilterChili.Extensions;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace GravityCTRL.FilterChili.Resolvers
{
    public abstract class DomainResolver
    {
        public string Name { get; protected set; }

        protected DomainResolver(string name)
        {
            Name = name;
        }
    }

    public abstract class DomainResolver<TSource> : DomainResolver
    {
        private readonly Type _sourceType;
        private readonly Type _selectorType;

        internal abstract bool NeedsToBeResolved { get; set; }

        [UsedImplicitly]
        public abstract string FilterType { get; }

        [UsedImplicitly]
        public string SourceType => _sourceType.Name;

        [UsedImplicitly]
        public string TargetType => _selectorType.Name;

        protected DomainResolver(string name, Type type) : base(name)
        {
            _sourceType = typeof(TSource);
            _selectorType = type;
        }

        public abstract bool TrySet(JToken domainToken);
    }

    public abstract class DomainResolver<TSource, TSelector> : DomainResolver<TSource> where TSelector : IComparable
    {
        protected Expression<Func<TSource, TSelector>> Selector { get; }

        protected internal DomainResolver(Expression<Func<TSource, TSelector>> selector) : base(selector.Name(), typeof(TSelector))
        {
            Selector = selector;
        }

        internal async Task SetAvailableEntities(IQueryable<TSource> queryable)
        {
            await SetAvailableValues(queryable.Select(Selector));
        }

        internal async Task SetSelectableEntities(IQueryable<TSource> queryable)
        {
            await SetSelectableValues(queryable.Select(Selector));
        }

        internal IQueryable<TSource> ExecuteFilter(IQueryable<TSource> queryable)
        {
            var expression = FilterExpression();
            return expression == null
                ? queryable
                : queryable.Where(expression);
        }

        protected abstract Task SetAvailableValues(IQueryable<TSelector> allValues);

        protected abstract Task SetSelectableValues(IQueryable<TSelector> selectableItems);

        protected abstract Expression<Func<TSource, bool>> FilterExpression();
    }

    public abstract class DomainResolver<TDomainResolver, TSource, TSelector> : DomainResolver<TSource, TSelector>
        where TSelector : IComparable where TDomainResolver : DomainResolver<TDomainResolver, TSource, TSelector>
    {
        protected DomainResolver(Expression<Func<TSource, TSelector>> selector) : base(selector) {}

        public TDomainResolver UseName(string name)
        {
            Name = name;
            return (TDomainResolver)this;
        }
    }
}