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

            var expressions = CreateExpressions(interpretedSearch);
            var searchExpression = expressions.And();
            if (!searchExpression.TryGetValue(out var searches))
            {
                return Option.None<Expression<Func<TSource, bool>>>();
            }

            var rewrittenExpression = PredicateRewriter.Rewrite(ParameterExpression, searches);
            return Option.Some(Expression.Lambda<Func<TSource, bool>>(rewrittenExpression, ParameterExpression));
        }

        [ItemNotNull]
        private IEnumerable<Expression> CreateExpressions([NotNull] InterpretedSearch interpretedSearch)
        {
            foreach (var constrainedIncludeExpression in CreateConstrainedIncludeExpressions(interpretedSearch)) yield return constrainedIncludeExpression;
            foreach (var constrainedExcludeExpression in CreateConstrainedExcludeExpressions(interpretedSearch)) yield return constrainedExcludeExpression;
            if (CreateIncludeExpression(interpretedSearch).TryGetValue(out var includeExpression)) yield return includeExpression;
            if (CreateExcludeExpression(interpretedSearch).TryGetValue(out var excludeExpression)) yield return excludeExpression;
        }

        [ItemNotNull]
        private IEnumerable<Expression> CreateConstrainedExcludeExpressions([NotNull] InterpretedSearch interpretedSearch)
        {
            var constrainedExcludeFragments = interpretedSearch.Where(fragment => fragment is ConstrainedExcludeFragment).Cast<ConstrainedExcludeFragment>().ToList();
            if (!constrainedExcludeFragments.Any())
            {
                yield break;
            }

            var constrainedExcludeGroups = constrainedExcludeFragments.GroupBy(fragment => fragment.PropertyName);
            foreach (var constrainedIncludeGroup in constrainedExcludeGroups)
            {
                var requestedSearcher = _searchers.SingleOrDefault(searcher =>
                    searcher.Names.Any(name => string.Equals(constrainedIncludeGroup.Key, name, StringComparison.InvariantCultureIgnoreCase))
                );

                if (requestedSearcher == null)
                {
                    continue;
                }

                var orExpression = constrainedIncludeGroup.Select(value => requestedSearcher.ExcludeExpression(value.Text)).Or();
                if (orExpression.TryGetValue(out var or))
                {
                    yield return Expression.Not(or);
                }
            }
        }

        [ItemNotNull]
        private IEnumerable<Expression> CreateConstrainedIncludeExpressions([NotNull] InterpretedSearch interpretedSearch)
        {
            var constrainedIncludeFragments = interpretedSearch.Where(fragment => fragment is ConstrainedIncludeFragment).Cast<ConstrainedIncludeFragment>().ToList();
            if (!constrainedIncludeFragments.Any())
            {
                yield break;
            }

            var constrainedIncludeGroups = constrainedIncludeFragments.GroupBy(fragment => fragment.PropertyName);
            foreach (var constrainedIncludeGroup in constrainedIncludeGroups)
            {
                var requestedSearcher = _searchers.SingleOrDefault(searcher =>
                    searcher.Names.Any(name => string.Equals(constrainedIncludeGroup.Key, name, StringComparison.InvariantCultureIgnoreCase))
                );

                if (requestedSearcher == null)
                {
                    continue;
                }

                var orExpression = constrainedIncludeGroup.Select(value => requestedSearcher.IncludeExpression(value.Text)).Or();
                if (orExpression.TryGetValue(out var or))
                {
                    yield return or;
                }
            }
        }

        [NotNull]
        private Option<UnaryExpression> CreateExcludeExpression([NotNull] InterpretedSearch interpretedSearch)
        {
            var excludeFragments = interpretedSearch.Where(fragment => fragment is ExcludeFragment).Cast<ExcludeFragment>().ToList();
            if (!excludeFragments.Any())
            {
                return Option.None<UnaryExpression>();
            }

            var expression = ExcludeExpressions(_searchers, excludeFragments).Or();
            return expression.TryGetValue(out var excludes) 
                ? Option.Some(Expression.Not(excludes)) 
                : Option.None<UnaryExpression>();
        }

        [NotNull]
        private Option<Expression> CreateIncludeExpression([NotNull] InterpretedSearch interpretedSearch)
        {
            var includeFragments = interpretedSearch.Where(fragment => fragment is IncludeFragment).Cast<IncludeFragment>().ToList();
            if (!includeFragments.Any())
            {
                return Option.None<Expression>();
            }

            var expression = IncludeExpressions(_searchers, includeFragments).Or();
            return expression.TryGetValue(out var includes) 
                ? Option.Some(includes) 
                : Option.None<Expression>();
        }

        [ItemNotNull]
        private static IEnumerable<Expression> IncludeExpressions([NotNull] IReadOnlyCollection<SearchSpecification<TSource>> usedSearchers, [NotNull] IReadOnlyCollection<IncludeFragment> includeFragments)
        {
            foreach (var searcher in usedSearchers)
            {
                if (searcher.IsDisabledForGeneralRequests)
                {
                    continue;
                }

                var includeExpressionGroups = includeFragments
                    .GroupBy(fragment => fragment.GroupId)
                    .Select(group => group.Select(fragment => searcher.IncludeExpression(fragment.Text)).Or())
                    .SelectValues()
                    .ToList();

                if (searcher.IncludeAcceptsMultipleInputs)
                {
                    if (includeExpressionGroups.And().TryGetValue(out var includes))
                    {
                        yield return includes;
                    }
                }
                else if (includeExpressionGroups.Count == 1)
                {
                    yield return includeExpressionGroups.First();
                }
            }
        }

        [ItemNotNull]
        private static IEnumerable<Expression> ExcludeExpressions([NotNull] IReadOnlyCollection<SearchSpecification<TSource>> usedSearchers, [NotNull] IReadOnlyCollection<ExcludeFragment> excludeFragments)
        {
            foreach (var searcher in usedSearchers)
            {
                if (searcher.IsDisabledForGeneralRequests)
                {
                    continue;
                }

                var excludeExpressions = excludeFragments.Select(fragment => searcher.ExcludeExpression(fragment.Text));
                if (excludeExpressions.Or().TryGetValue(out var orExpression))
                {
                    yield return orExpression;
                }
            }
        }
    }
}
