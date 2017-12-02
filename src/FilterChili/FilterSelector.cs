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
using GravityCTRL.FilterChili.Providers;
using GravityCTRL.FilterChili.Resolvers;
using GravityCTRL.FilterChili.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GravityCTRL.FilterChili
{
    public abstract class FilterSelector<TSource>
    {
        #region Internal Methods

        internal abstract IQueryable<TSource> ApplyFilter(IQueryable<TSource> queryable);

        internal abstract Task Resolve(IQueryable<TSource> queryable, IQueryable<TSource> selectableItems);

        internal abstract DomainResolver<TSource> Domain();

        internal abstract bool HasName(string name);

        internal abstract bool TrySet<TSelector>(TSelector min, TSelector max);

        internal abstract bool TrySet<TSelector>(IEnumerable<TSelector> values);

        internal abstract bool TrySet(JToken domainToken);

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

        internal override bool HasName(string name)
        {
            return _domainResolver.Name == name;
        }

        internal override bool TrySet(JToken domainToken)
        {
            try
            {
                switch (_domainResolver)
                {
                    case RangeResolver<TSource, TSelector> range:
                    {
                        var domain = domainToken.ToObject<Range<TSelector>>(JsonUtils.Serializer);
                        range.Set(domain.Min, domain.Max);
                        return true;
                    }
                    case ListResolver<TSource, TSelector> list:
                    {
                        var domain = domainToken.ToObject<Set<TSelector>>(JsonUtils.Serializer);
                        list.Set(domain.Values);
                        return true;
                    }
                    default:
                    {
                        return false;
                    }
                }
            }
            catch (JsonSerializationException)
            {
                return false;
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

    public class StringFilterSelector<TSource> : FilterSelector<TSource, string, StringDomainProvider<TSource>>
    {
        internal StringFilterSelector(Action onChange, Expression<Func<TSource, string>> valueSelector) : base(new StringDomainProvider<TSource>(onChange, valueSelector)) { }
    }

    public class IntFilterSelector<TSource> : FilterSelector<TSource, int, IntDomainProvider<TSource>>
    {
        internal IntFilterSelector(Action onChange, Expression<Func<TSource, int>> valueSelector) : base(new IntDomainProvider<TSource>(onChange, valueSelector)) { }
    }
}