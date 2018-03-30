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
using GravityCTRL.FilterChili.Search.Fragments;
using JetBrains.Annotations;

namespace GravityCTRL.FilterChili.Search
{
    internal sealed class SearchResolver<TSource>
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly ParameterExpression ParameterExpression;

        private readonly List<SearchSelector<TSource>> _searchers;

        [CanBeNull]
        private string _searchString;

        [CanBeNull]
        private Expression<Func<TSource, bool>> _searchExpression;

        static SearchResolver()
        {
            ParameterExpression = Expression.Parameter(typeof(TSource));
        }

        public SearchResolver()
        {
            _searchers = new List<SearchSelector<TSource>>();
        }

        public IQueryable<TSource> ApplySearch(IQueryable<TSource> queryable)
        {
            return _searchExpression == null ? queryable : queryable.Where(_searchExpression);
        }

        public void SetSearchString(string searchString)
        {
            searchString = searchString ?? string.Empty;
            if (string.Equals(_searchString, searchString))
            {
                return;
            }

            _searchString = searchString;
            var fragmentedSearch = new FragmentedSearch(searchString);
            _searchExpression = SetSearchExpression(fragmentedSearch);
        }

        internal void AddSearcher(SearchSelector<TSource> searchSelector)
        {
            _searchers.Add(searchSelector);
        }

        [CanBeNull]
        private Expression<Func<TSource, bool>> SetSearchExpression(FragmentedSearch fragmentedSearch)
        {
            if (!_searchers.Any())
            {
                return null;
            }

            var expressions = new List<Expression>();

            var constrainedIncludeFragments = fragmentedSearch.Where(fragment => fragment is ConstrainedIncludeFragment).Cast<ConstrainedIncludeFragment>().ToList();
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

            var constrainedExcludeFragments = fragmentedSearch.Where(fragment => fragment is ConstrainedExcludeFragment).Cast<ConstrainedExcludeFragment>().ToList();
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

            var includeFragments = fragmentedSearch.Where(fragment => fragment is IncludeFragment).Cast<IncludeFragment>().ToList();
            if (includeFragments.Any())
            {
                var expression = IncludeExpressions(_searchers, includeFragments);
                if (expression != null)
                {
                    expressions.Add(expression);
                }
            }

            var excludeFragments = fragmentedSearch.Where(fragment => fragment is ExcludeFragment).Cast<ExcludeFragment>().ToList();
            if (excludeFragments.Any())
            {
                var expression = ExcludeExpressions(_searchers, excludeFragments);
                if (expression != null)
                {
                    expressions.Add(expression);
                }
            }

            var searchExpression = expressions.And();
            if (searchExpression == null)
            {
                return null;
            }

            var rewrittenExpression = PredicateRewriter.Rewrite(ParameterExpression, searchExpression);
            return Expression.Lambda<Func<TSource, bool>>(rewrittenExpression, ParameterExpression);
        }

        [CanBeNull]
        private static Expression IncludeExpressions([NotNull] IReadOnlyCollection<SearchSelector<TSource>> usedSearchers, [NotNull] IReadOnlyCollection<IncludeFragment> includeFragments)
        {
            IEnumerable<Expression> CreateIncludeExpressions()
            {
                foreach (var searcher in usedSearchers)
                {
                    if (searcher.IncludeAcceptsMultipleInputs)
                    {
                        var includeExpressions = includeFragments.Select(fragment => searcher.IncludeExpression(fragment.Text));
                        yield return includeExpressions.And();

                    }
                    else if (includeFragments.Count == 1)
                    {
                        yield return searcher.IncludeExpression(includeFragments.First().Text);
                    }
                }
            }

            return CreateIncludeExpressions().Or();
        }

        [CanBeNull]
        private static Expression ExcludeExpressions([NotNull] IReadOnlyCollection<SearchSelector<TSource>> usedSearchers, [NotNull] IReadOnlyCollection<ExcludeFragment> excludeFragments)
        {
            IEnumerable<Expression> CreateExcludeExpressions()
            {
                foreach (var searcher in usedSearchers)
                {
                    var excludeExpressions = excludeFragments.Select(fragment => searcher.ExcludeExpression(fragment.Text));
                    yield return excludeExpressions.Or();
                }
            }

            var expression = CreateExcludeExpressions().Or();
            return expression == null ? null : Expression.Not(expression);
        }
    }
}
