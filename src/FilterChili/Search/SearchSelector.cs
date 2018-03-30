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
using System.Linq.Expressions;
using GravityCTRL.FilterChili.Expressions;
using GravityCTRL.FilterChili.Extensions;
using GravityCTRL.FilterChili.Phonetics;
using JetBrains.Annotations;

namespace GravityCTRL.FilterChili.Search
{
    public sealed class SearchSelector<TSource>
    {
        private readonly Expression<Func<TSource, string>> _selector;
        private IExpressionProvider<TSource> _includeExpressionProvider;
        private IExpressionProvider<TSource> _excludeExpressionProvider;

        [UsedImplicitly]
        public string Name { get; private set; }

        internal bool IncludeAcceptsMultipleInputs => _includeExpressionProvider.AcceptsMultipleSearchInputs;

        internal SearchSelector([NotNull] Expression<Func<TSource, string>> valueSelector)
        {
            Name = valueSelector.Name();
            _selector = valueSelector;
            UseContains();
        }

        internal Expression IncludeExpression(string search)
        {
            return _includeExpressionProvider.SearchExpression(_selector, search);
        }

        internal Expression ExcludeExpression(string search)
        {
            return _excludeExpressionProvider.SearchExpression(_selector, search);
        }

        [NotNull]
        [UsedImplicitly]
        public SearchSelector<TSource> UseEquals()
        {
            _includeExpressionProvider = new EqualsExpressionProvider<TSource>();
            _excludeExpressionProvider = new EqualsExpressionProvider<TSource>();
            return this;
        }

        [NotNull]
        [UsedImplicitly]
        public SearchSelector<TSource> UseContains()
        {
            _includeExpressionProvider = new ContainsExpressionProvider<TSource>();
            _excludeExpressionProvider = new ContainsExpressionProvider<TSource>();
            return this;
        }

        [NotNull]
        [UsedImplicitly]
        public SearchSelector<TSource> UseSoundex()
        {
            _includeExpressionProvider = new SoundexExpressionProvider<TSource>();
            _excludeExpressionProvider = new ContainsExpressionProvider<TSource>();
            return this;
        }

        [NotNull]
        [UsedImplicitly]
        public SearchSelector<TSource> UseGermanSoundex()
        {
            _includeExpressionProvider = new GermanSoundexExpressionProvider<TSource>();
            _excludeExpressionProvider = new ContainsExpressionProvider<TSource>();
            return this;
        }

        [NotNull]
        [UsedImplicitly]
        public SearchSelector<TSource> UseName(string name)
        {
            Name = name;
            return this;
        }
    }

    internal interface IExpressionProvider<TSource>
    {
        bool AcceptsMultipleSearchInputs { get; }

        Expression SearchExpression([NotNull] Expression<Func<TSource, string>> searchSelector, string search);
    }

    internal sealed class EqualsExpressionProvider<TSource> : IExpressionProvider<TSource>
    {
        public bool AcceptsMultipleSearchInputs { get; } = false;

        public Expression SearchExpression(Expression<Func<TSource, string>> searchSelector, string search)
        {
            var constant = Expression.Constant(search);
            return Expression.Equal(Expression.Call(searchSelector.Body, MethodExpressions.ToLowerExpression), constant);
        }
    }

    internal sealed class ContainsExpressionProvider<TSource> : IExpressionProvider<TSource>
    {
        public bool AcceptsMultipleSearchInputs { get; } = true;

        public Expression SearchExpression(Expression<Func<TSource, string>> searchSelector, string search)
        {
            var constant = Expression.Constant(search);
            return Expression.Call(Expression.Call(searchSelector.Body, MethodExpressions.ToLowerExpression), MethodExpressions.StringContainsExpression, constant);
        }
    }

    internal sealed class SoundexExpressionProvider<TSource> : IExpressionProvider<TSource>
    {
        public bool AcceptsMultipleSearchInputs { get; } = true;

        public Expression SearchExpression(Expression<Func<TSource, string>> searchSelector, string search)
        {
            var compiledExpression = searchSelector.Compile();
            Expression<Func<TSource, bool>> expression = entity => search.ToSoundex().Contains(compiledExpression(entity).ToSoundex());
            return expression.Body;
        }
    }

    internal sealed class GermanSoundexExpressionProvider<TSource> : IExpressionProvider<TSource>
    {
        public bool AcceptsMultipleSearchInputs { get; } = true;

        public Expression SearchExpression(Expression<Func<TSource, string>> searchSelector, string search)
        {
            var compiledExpression = searchSelector.Compile();
            Expression<Func<TSource, bool>> expression = entity => search.ToGermanSoundex().Contains(compiledExpression(entity).ToGermanSoundex());
            return expression.Body;
        }
    }
}
