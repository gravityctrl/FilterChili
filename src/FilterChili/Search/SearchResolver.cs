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
using GravityCTRL.FilterChili.Expressions;
using GravityCTRL.FilterChili.Extensions;
using GravityCTRL.FilterChili.Models;
using GravityCTRL.FilterChili.Search.Fragments;
using JetBrains.Annotations;

namespace GravityCTRL.FilterChili.Search
{
    internal sealed class SearchResolver<TSource>
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly ParameterExpression ParameterExpression;

        private readonly List<SearchSpecification<TSource>> _searchers;

        [CanBeNull]
        private string _searchString;

        [NotNull]
        private Option<Expression<Func<TSource, bool>>> _searchExpression;

        static SearchResolver()
        {
            ParameterExpression = Expression.Parameter(typeof(TSource));
        }

        public SearchResolver()
        {
            _searchers = new List<SearchSpecification<TSource>>();
            _searchExpression = Option.None<Expression<Func<TSource, bool>>>();
        }

        public IQueryable<TSource> ApplySearch(IQueryable<TSource> queryable)
        {
            return _searchExpression.TryGetValue(out var value) 
                ? queryable.Where(value) 
                : queryable;
        }

        public void SetSearchString(string searchString)
        {
            searchString = searchString ?? string.Empty;
            if (string.Equals(_searchString, searchString))
            {
                return;
            }

            _searchString = searchString;
            var interpretedSearch = new InterpretedSearch(searchString);
            _searchExpression = SetSearchExpression(interpretedSearch);
        }

        internal void AddSearcher(SearchSpecification<TSource> searchSpecification)
        {
            _searchers.Add(searchSpecification);
        }

        [NotNull]
        private Option<Expression<Func<TSource, bool>>> SetSearchExpression(InterpretedSearch interpretedSearch)
        {
            if (!_searchers.Any())
            {
                return Option.None<Expression<Func<TSource, bool>>>();
            }

            var expressions = new List<Expression>();

            var constrainedIncludeFragments = interpretedSearch.Where(fragment => fragment is ConstrainedIncludeFragment).Cast<ConstrainedIncludeFragment>().ToList();
            if (constrainedIncludeFragments.Any())
            {
                foreach (var fragment in constrainedIncludeFragments)
                {
                    var requestedSearcher = _searchers.SingleOrDefault(searcher => string.Equals(fragment.PropertyName, searcher.Name, StringComparison.InvariantCultureIgnoreCase));
                    if (requestedSearcher == null)
                    {
                        continue;
                    }

                    var expression = requestedSearcher.IncludeExpression(fragment.Text);
                    expressions.Add(expression);
                }
            }

            var constrainedExcludeFragments = interpretedSearch.Where(fragment => fragment is ConstrainedExcludeFragment).Cast<ConstrainedExcludeFragment>().ToList();
            if (constrainedExcludeFragments.Any())
            {
                foreach (var fragment in constrainedExcludeFragments)
                {
                    var requestedSearcher = _searchers.SingleOrDefault(searcher => string.Equals(fragment.PropertyName, searcher.Name, StringComparison.InvariantCultureIgnoreCase));
                    if (requestedSearcher == null)
                    {
                        continue;
                    }

                    var expression = Expression.Not(requestedSearcher.ExcludeExpression(fragment.Text));
                    expressions.Add(expression);
                }
            }

            var includeFragments = interpretedSearch.Where(fragment => fragment is IncludeFragment).Cast<IncludeFragment>().ToList();
            if (includeFragments.Any())
            {
                var expression = IncludeExpressions(_searchers, includeFragments);
                if (expression.TryGetValue(out var includes))
                {
                    expressions.Add(includes);
                }
            }

            var excludeFragments = interpretedSearch.Where(fragment => fragment is ExcludeFragment).Cast<ExcludeFragment>().ToList();
            if (excludeFragments.Any())
            {
                var expression = ExcludeExpressions(_searchers, excludeFragments);
                if (expression.TryGetValue(out var excludes))
                {
                    expressions.Add(excludes);
                }
            }

            var searchExpression = expressions.And();
            if (!searchExpression.TryGetValue(out var searches))
            {
                return Option.None<Expression<Func<TSource, bool>>>();
            }

            var rewrittenExpression = PredicateRewriter.Rewrite(ParameterExpression, searches);
            return Option.Some(Expression.Lambda<Func<TSource, bool>>(rewrittenExpression, ParameterExpression));
        }

        [NotNull]
        private static Option<Expression> IncludeExpressions([NotNull] IReadOnlyCollection<SearchSpecification<TSource>> usedSearchers, [NotNull] IReadOnlyCollection<IncludeFragment> includeFragments)
        {
            IEnumerable<Expression> CreateIncludeExpressions()
            {
                foreach (var searcher in usedSearchers)
                {
                    if (searcher.IncludeAcceptsMultipleInputs)
                    {
                        var includeExpressions = includeFragments.Select(fragment => searcher.IncludeExpression(fragment.Text));
                        if (includeExpressions.And().TryGetValue(out var includes))
                        {
                            yield return includes;
                        }
                    }
                    else if (includeFragments.Count == 1)
                    {
                        yield return searcher.IncludeExpression(includeFragments.First().Text);
                    }
                }
            }

            return CreateIncludeExpressions().Or();
        }

        [NotNull]
        private static Option<UnaryExpression> ExcludeExpressions([NotNull] IReadOnlyCollection<SearchSpecification<TSource>> usedSearchers, [NotNull] IReadOnlyCollection<ExcludeFragment> excludeFragments)
        {
            IEnumerable<Expression> CreateExcludeExpressions()
            {
                foreach (var searcher in usedSearchers)
                {
                    var excludeExpressions = excludeFragments.Select(fragment => searcher.ExcludeExpression(fragment.Text));
                    if (excludeExpressions.Or().TryGetValue(out var orExpression))
                    {
                        yield return orExpression;
                    }
                }
            }

            var expression = CreateExcludeExpressions().Or();
            return expression.TryGetValue(out var value) 
                ? Option.Some(Expression.Not(value)) 
                : Option.None<UnaryExpression>();
        }
    }
}
