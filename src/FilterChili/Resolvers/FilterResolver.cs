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
using GravityCTRL.FilterChili.Models;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace GravityCTRL.FilterChili.Resolvers
{
    public abstract class FilterResolver
    {
        [UsedImplicitly]
        public string Name { get; protected set; }

        internal CalculationStrategy CalculationStrategy { get; set; }

        internal FilterResolver(string name)
        {
            Name = name;
            CalculationStrategy = CalculationStrategy.Full;
        }
    }

    public abstract class FilterResolver<TSource> : FilterResolver
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

        static FilterResolver()
        {
            GenericSourceType = typeof(TSource);
        }

        internal FilterResolver(string name, Type type) : base(name)
        {
            _selectorType = type;
        }

        public abstract bool TrySet(JToken filterToken);
    }

    public abstract class FilterResolver<TSource, TValue> : FilterResolver<TSource> where TValue : IComparable
    {
        protected Expression<Func<TSource, TValue>> Selector { get; }

        internal FilterResolver([NotNull] Expression<Func<TSource, TValue>> selector) : base(selector.Name(), typeof(TValue))
        {
            Selector = selector;
        }

        internal IQueryable<TSource> ExecuteFilter(IQueryable<TSource> queryable)
        {
            var expression = FilterExpression();
            return expression.TryGetValue(out var value)
                ? queryable.Where(value)
                : queryable;
        }

        internal abstract Task SetEntities([NotNull] Option<IQueryable<TSource>> allEntities, [NotNull] Option<IQueryable<TSource>> selectableEntities);

        [NotNull]
        internal abstract Option<Expression<Func<TSource, bool>>> FilterExpression();
    }

    public abstract class FilterResolver<TFilterResolver, TSource, TValue> : FilterResolver<TSource, TValue>
        where TValue : IComparable where TFilterResolver : FilterResolver<TFilterResolver, TSource, TValue>
    {
        private readonly TFilterResolver _this;

        internal FilterResolver([NotNull] Expression<Func<TSource, TValue>> selector) : base(selector)
        {
            _this = (TFilterResolver)this;
        }

        [UsedImplicitly]
        public TFilterResolver UseName([NotNull] string name)
        {
            Name = name.Trim();
            return _this;
        }

        [UsedImplicitly]
        public TFilterResolver UseStrategy(CalculationStrategy calculationStrategy)
        {
            CalculationStrategy = calculationStrategy;
            return _this;
        }
    }
}