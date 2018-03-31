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
        [UsedImplicitly]
        public string Name { get; protected set; }

        internal CalculationStrategy CalculationStrategy { get; set; }

        internal DomainResolver(string name)
        {
            Name = name;
            CalculationStrategy = CalculationStrategy.Full;
        }
    }

    public abstract class DomainResolver<TSource> : DomainResolver
    {
        // ReSharper disable once StaticMemberInGenericType
        protected static readonly Type GenericSourceType;
        private readonly Type _selectorType;

        internal abstract bool NeedsToBeResolved { get; set; }

        [UsedImplicitly]
        public abstract string FilterType { get; }

        [UsedImplicitly]
        public string SourceType => GenericSourceType.Name;

        [UsedImplicitly]
        public string TargetType => _selectorType.Name;

        static DomainResolver()
        {
            GenericSourceType = typeof(TSource);
        }

        internal DomainResolver(string name, Type type) : base(name)
        {
            _selectorType = type;
        }

        public abstract bool TrySet(JToken domainToken);
    }

    public abstract class DomainResolver<TSource, TSelector> : DomainResolver<TSource> where TSelector : IComparable
    {
        protected Expression<Func<TSource, TSelector>> Selector { get; }

        internal DomainResolver([NotNull] Expression<Func<TSource, TSelector>> selector) : base(selector.Name(), typeof(TSelector))
        {
            Selector = selector;
        }

        internal IQueryable<TSource> ExecuteFilter(IQueryable<TSource> queryable)
        {
            var expression = FilterExpression();
            return expression == null
                ? queryable
                : queryable.Where(expression);
        }

        internal abstract Task SetAvailableEntities(IQueryable<TSource> queryable);

        internal abstract Task SetSelectableEntities(IQueryable<TSource> queryable);

        [CanBeNull]
        protected abstract Expression<Func<TSource, bool>> FilterExpression();
    }

    public abstract class DomainResolver<TDomainResolver, TSource, TSelector> : DomainResolver<TSource, TSelector>
        where TSelector : IComparable where TDomainResolver : DomainResolver<TDomainResolver, TSource, TSelector>
    {
        private readonly TDomainResolver _this;

        internal DomainResolver([NotNull] Expression<Func<TSource, TSelector>> selector) : base(selector)
        {
            _this = (TDomainResolver)this;
        }

        [UsedImplicitly]
        public TDomainResolver UseName(string name)
        {
            Name = name;
            return _this;
        }

        [UsedImplicitly]
        public TDomainResolver UseCalculationStrategy(CalculationStrategy calculationStrategy)
        {
            CalculationStrategy = calculationStrategy;
            return _this;
        }
    }
}