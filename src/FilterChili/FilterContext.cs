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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GravityCTRL.FilterChili.Enums;
using GravityCTRL.FilterChili.Resolvers;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace GravityCTRL.FilterChili
{
    public abstract class FilterContext<TSource>
    {
        private readonly ContextOptions<TSource> _contextOptions;

        protected FilterContext(IQueryable<TSource> queryable)
        {
            _contextOptions = new ContextOptions<TSource>(queryable, Configure);
        }

        [UsedImplicitly]
        public IQueryable<TSource> ApplyFilters()
        {
            return _contextOptions.ApplyFilters();
        }

        [UsedImplicitly]
        public async Task<IEnumerable<DomainResolver<TSource>>> Domains()
        {
            return await _contextOptions.Domains();
        }

        [UsedImplicitly]
        public async Task<IEnumerable<DomainResolver<TSource>>> Domains(CalculationStrategy calculationStrategy)
        {
            return await _contextOptions.Domains(calculationStrategy);
        }

        [UsedImplicitly]
        public bool TrySet(JArray filterTokens)
        {
            return filterTokens.All(TrySet);
        }

        [UsedImplicitly]
        public bool TrySet(JToken filterToken)
        {
            var name = filterToken.Value<string>("name");
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            var filter = _contextOptions.GetFilter(name);
            if (filter == null)
            {
                return false;
            }

            var domain = filterToken.SelectToken("domain");
            return domain != null && filter.TrySet(domain);
        }

        [UsedImplicitly]
        public bool TrySet<TSelector>(string name, TSelector value)
        {
            return _contextOptions.GetFilter(name)?.TrySet(value) ?? false;
        }

        [UsedImplicitly]
        public bool TrySet<TSelector>(string name, TSelector min, TSelector max)
        {
            return _contextOptions.GetFilter(name)?.TrySet(min, max) ?? false;
        }

        [UsedImplicitly]
        public bool TrySet<TSelector>(string name, IEnumerable<TSelector> values)
        {
            return _contextOptions.GetFilter(name)?.TrySet(values) ?? false;
        }

        protected abstract void Configure(ContextOptions<TSource> options);
    }
}
