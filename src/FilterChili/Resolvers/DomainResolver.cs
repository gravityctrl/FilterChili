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

namespace GravityCTRL.FilterChili.Resolvers
{
    public abstract class DomainResolver<TSource>
    {
        private readonly Type _sourceType;
        private readonly Type _selectorType;

        public string Name { get; }
        public string SourceType => _sourceType.Name;
        public string TargetType => _selectorType.Name;

        internal abstract Task Resolve(IQueryable<TSource> queryable, IQueryable<TSource> selectableItems);
        internal abstract IQueryable<TSource> ExecuteFilter(IQueryable<TSource> queryable);

        protected DomainResolver(string name, Type type)
        {
            _sourceType = typeof(TSource);
            _selectorType = type;
            Name = name;
        }
    }

    public abstract class DomainResolver<TSource, TSelector> : DomainResolver<TSource>
    {
        protected internal readonly Expression<Func<TSource, TSelector>> Selector;

        protected internal DomainResolver(string name, Expression<Func<TSource, TSelector>> selector) : base(name, typeof(TSelector))
        {
            Selector = selector;
        }

        internal override async Task Resolve(IQueryable<TSource> queryable, IQueryable<TSource> selectableItems)
        {
            await Resolve(queryable.Select(Selector), selectableItems.Select(Selector));
        }

        internal override IQueryable<TSource> ExecuteFilter(IQueryable<TSource> queryable)
        {
            var expression = FilterExpression();
            return expression == null 
                ? queryable 
                : queryable
                    .GroupBy(Selector)
                    .Where(FilterExpression())
                    .SelectMany(group => group);
        }

        internal abstract Task Resolve(IQueryable<TSelector> allItems, IQueryable<TSelector> selectableItems);

        internal abstract Expression<Func<IGrouping<TSelector, TSource>, bool>> FilterExpression();
    }
}