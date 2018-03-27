﻿// This file is part of FilterChili.
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
using GravityCTRL.FilterChili.Behaviors;
using GravityCTRL.FilterChili.Extensions;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace GravityCTRL.FilterChili.Resolvers
{
    public abstract class DomainResolver
    {
        public string Name { get; internal set; }

        protected DomainResolver(string name)
        {
            Name = name;
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

        protected DomainResolver(string name, Type type) : base(name)
        {
            _selectorType = type;
        }

        public abstract bool TrySet(JToken domainToken);

        internal abstract void ApplyBehaviors();
    }

    public abstract class DomainResolver<TSource, TSelector> : DomainResolver<TSource> where TSelector : IComparable
    {
        protected Expression<Func<TSource, TSelector>> Selector { get; }

        protected internal DomainResolver(Expression<Func<TSource, TSelector>> selector) : base(selector.Name(), typeof(TSelector))
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

        protected abstract Expression<Func<TSource, bool>> FilterExpression();
    }

    public abstract class DomainResolver<TDomainResolver, TSource, TSelector> : DomainResolver<TSource, TSelector>
        where TSelector : IComparable where TDomainResolver : DomainResolver<TDomainResolver, TSource, TSelector>
    {
        private readonly List<IBehavior<TSource, TSelector>> _behaviors;
        private readonly TDomainResolver _this;

        protected DomainResolver(Expression<Func<TSource, TSelector>> selector) : base(selector)
        {
            _behaviors = new List<IBehavior<TSource, TSelector>>();
            _this = (TDomainResolver)this;
        }

        public TDomainResolver UseName(string name)
        {
            var behavior = new ReplaceNameBehavior<TSource, TSelector>(name);
            AddBehavior(behavior);
            return _this;
        }

        internal override void ApplyBehaviors()
        {
            var resolver = (TDomainResolver)this;
            _behaviors.ForEach(behavior => behavior.Apply(resolver));
        }

        private void AddBehavior<TBehavior>(TBehavior behavior) where TBehavior : IBehavior<TSource, TSelector>
        {
            var index = _behaviors.FindIndex(existingBehavior => existingBehavior is TBehavior);
            if (index > -1)
            {
                _behaviors[index] = behavior;
            }
            else
            {
                _behaviors.Add(behavior);
            }
        }
    }
}