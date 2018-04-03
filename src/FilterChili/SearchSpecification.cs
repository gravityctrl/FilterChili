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
using GravityCTRL.FilterChili.Extensions;
using GravityCTRL.FilterChili.Models;
using GravityCTRL.FilterChili.Search.ExpressionProviders;
using JetBrains.Annotations;

namespace GravityCTRL.FilterChili
{
    public sealed class SearchSpecification<TSource>
    {
        internal bool IsDisabledForGeneralRequests { get; private set; }

        private readonly Expression<Func<TSource, string>> _selector;
        private IExpressionProvider<TSource> _includeExpressionProvider;
        private IExpressionProvider<TSource> _excludeExpressionProvider;
        private Option<IReadOnlyList<string>> _aliases;

        [UsedImplicitly]
        public string Name { get; private set; }

        [NotNull]
        internal IReadOnlyList<string> Names => _aliases.TryGetValue(out var aliases) ? Name.Append(aliases).ToArray() : new[] { Name };

        internal bool IncludeAcceptsMultipleInputs => _includeExpressionProvider.AcceptsMultipleSearchInputs;

        internal SearchSpecification([NotNull] Expression<Func<TSource, string>> valueSelector)
        {
            _aliases = Option.None<IReadOnlyList<string>>();
            Name = valueSelector.Name();
            _selector = valueSelector;
            UseContains();
        }

        [NotNull]
        internal Expression IncludeExpression([NotNull] string search)
        {
            return _includeExpressionProvider.SearchExpression(_selector, search.ToLowerInvariant());
        }

        [NotNull]
        internal Expression ExcludeExpression([NotNull] string search)
        {
            return _excludeExpressionProvider.SearchExpression(_selector, search.ToLowerInvariant());
        }

        [NotNull]
        [UsedImplicitly]
        public SearchSpecification<TSource> UseEquals()
        {
            _includeExpressionProvider = new EqualsExpressionProvider<TSource>();
            _excludeExpressionProvider = new EqualsExpressionProvider<TSource>();
            return this;
        }

        [NotNull]
        [UsedImplicitly]
        public SearchSpecification<TSource> UseContains()
        {
            _includeExpressionProvider = new ContainsExpressionProvider<TSource>();
            _excludeExpressionProvider = new ContainsExpressionProvider<TSource>();
            return this;
        }

        [NotNull]
        [UsedImplicitly]
        public SearchSpecification<TSource> UseSoundex()
        {
            _includeExpressionProvider = new SoundexExpressionProvider<TSource>();
            _excludeExpressionProvider = new ContainsExpressionProvider<TSource>();
            return this;
        }

        [NotNull]
        [UsedImplicitly]
        public SearchSpecification<TSource> UseGermanSoundex()
        {
            _includeExpressionProvider = new GermanSoundexExpressionProvider<TSource>();
            _excludeExpressionProvider = new ContainsExpressionProvider<TSource>();
            return this;
        }

        [NotNull]
        [UsedImplicitly]
        public SearchSpecification<TSource> UseName([NotNull] string name)
        {
            Name = name.Trim();
            return this;
        }

        [NotNull]
        [UsedImplicitly]
        public SearchSpecification<TSource> UseAliases([NotNull] params string[] aliases)
        {
            IReadOnlyList<string> result = aliases.Select(alias => alias.Trim()).ToList();
            _aliases = Option.Some(result);
            return this;
        }

        [NotNull]
        [UsedImplicitly]
        public SearchSpecification<TSource> RequirePropertyMention()
        {
            IsDisabledForGeneralRequests = true;
            return this;
        }
    }
}
