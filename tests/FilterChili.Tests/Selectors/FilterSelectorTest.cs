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
using FluentAssertions;
using GravityCTRL.FilterChili.Comparison;
using GravityCTRL.FilterChili.Exceptions;
using GravityCTRL.FilterChili.Models;
using GravityCTRL.FilterChili.Selectors;
using GravityCTRL.FilterChili.Tests.TestSupport.Models;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using Xunit;

namespace GravityCTRL.FilterChili.Tests.Selectors
{
    public sealed class FilterSelectorTest
    {
        private readonly TestFilterSelector _testInstance;

        public FilterSelectorTest()
        {
            _testInstance = new TestFilterSelector(source => source.Int);
        }

        [Fact]
        public void Should_Throw_Exception_If_No_Resolver_Was_Specified()
        {
            var queryable = new GenericSource[0].AsQueryable();

            Action domainAction = () => _testInstance.Domain();
            domainAction.Should().Throw<MissingResolverException>().WithMessage(nameof(TestFilterSelector));

            Action applyFilterAction = () => _testInstance.ApplyFilter(queryable);
            applyFilterAction.Should().Throw<MissingResolverException>().WithMessage(nameof(TestFilterSelector));

            Action setAvailableEntitiesAction = () => _testInstance.SetEntities(Option.Some(queryable), Option.None<IQueryable<GenericSource>>()).Wait();
            setAvailableEntitiesAction.Should().Throw<MissingResolverException>().WithMessage(nameof(TestFilterSelector));

            Action setSelectableEntitiesAction = () => _testInstance.SetEntities(Option.None<IQueryable<GenericSource>>(), Option.Some(queryable)).Wait();
            setSelectableEntitiesAction.Should().Throw<MissingResolverException>().WithMessage(nameof(TestFilterSelector));

            Action needsToBeResolvedAction = () => _testInstance.NeedsToBeResolved = true;
            needsToBeResolvedAction.Should().Throw<MissingResolverException>().WithMessage(nameof(TestFilterSelector));
        }

        [Fact]
        public void Should_Not_Exception_If_No_Resolver_Was_Specified()
        {
            _testInstance.NeedsToBeResolved.Should().BeFalse();
            _testInstance.TrySet(1).Should().BeFalse();
            _testInstance.TrySet(1, 2).Should().BeFalse();
            _testInstance.TrySet(new [] { 1, 2, 3 }).Should().BeFalse();
            _testInstance.TrySet(JToken.Parse(@"{ ""min"": 1, ""max"": 2 }")).Should().BeFalse();
            _testInstance.HasName("TestName").Should().BeFalse();
            _testInstance.NeedsToBeResolved.Should().BeFalse();
        }

        [Fact]
        public void Shoud_Be_Able_To_Call_Filter_Methods_If_Resolver_Is_Set()
        {
            _testInstance.Comparison();

            var queryable = new GenericSource[0].AsQueryable();

            Action domainAction = () => _testInstance.Domain();
            domainAction.Should().NotThrow<MissingResolverException>();

            Action applyFilterAction = () => _testInstance.ApplyFilter(queryable);
            applyFilterAction.Should().NotThrow<MissingResolverException>();

            Action setAvailableEntitiesAction = () => _testInstance.SetEntities(Option.Some(queryable), Option.None<IQueryable<GenericSource>>()).Wait();
            setAvailableEntitiesAction.Should().NotThrow<MissingResolverException>();

            Action setSelectableEntitiesAction = () => _testInstance.SetEntities(Option.None<IQueryable<GenericSource>>(), Option.Some(queryable)).Wait();
            setSelectableEntitiesAction.Should().NotThrow<MissingResolverException>();

            Action needsToBeResolvedAction = () => _testInstance.NeedsToBeResolved = true;
            needsToBeResolvedAction.Should().NotThrow<MissingResolverException>();
        }

        [Fact]
        public void ComparisonResolver_Should_Be_Filled_With_Correct_TrySet_Method_Calls()
        {
            _testInstance.Comparison().Should().BeOfType<ComparisonResolver<GenericSource, int>>();
            _testInstance.Domain().Should().BeOfType<ComparisonResolver<GenericSource, int>>();
            _testInstance.NeedsToBeResolved = false;

            _testInstance.TrySet(1).Should().BeTrue();
            _testInstance.TrySet(1, 2).Should().BeFalse();
            _testInstance.TrySet(new[] { 1, 2, 3 }).Should().BeFalse();

            _testInstance.TrySet(JToken.Parse(@"{ ""value"": 1 }")).Should().BeTrue();
            _testInstance.TrySet(JToken.Parse(@"{ ""min"": 1, ""max"": 2 }")).Should().BeFalse();
            _testInstance.TrySet(JToken.Parse(@"{ ""values"": [ 1, 2, 3 ] }")).Should().BeFalse();

            _testInstance.NeedsToBeResolved.Should().BeTrue();
        }

        [Fact]
        public void RangeResolver_Should_Be_Filled_With_Correct_TrySet_Method_Calls()
        {
            _testInstance.Range().Should().BeOfType<RangeResolver<GenericSource, int>>();
            _testInstance.Domain().Should().BeOfType<RangeResolver<GenericSource, int>>();
            _testInstance.NeedsToBeResolved = false;

            _testInstance.TrySet(1).Should().BeFalse();
            _testInstance.TrySet(1, 2).Should().BeTrue();
            _testInstance.TrySet(new[] { 1, 2, 3 }).Should().BeFalse();

            _testInstance.TrySet(JToken.Parse(@"{ ""value"": 1 }")).Should().BeFalse();
            _testInstance.TrySet(JToken.Parse(@"{ ""min"": 1, ""max"": 2 }")).Should().BeTrue();
            _testInstance.TrySet(JToken.Parse(@"{ ""values"": [ 1, 2, 3 ] }")).Should().BeFalse();

            _testInstance.NeedsToBeResolved.Should().BeTrue();
        }

        [Fact]
        public void ListResolver_Should_Be_Filled_With_Correct_TrySet_Method_Calls()
        {
            _testInstance.List().Should().BeOfType<ListResolver<GenericSource, int>>();
            _testInstance.Domain().Should().BeOfType<ListResolver<GenericSource, int>>();
            _testInstance.NeedsToBeResolved = false;

            _testInstance.TrySet(1).Should().BeFalse();
            _testInstance.TrySet(1, 2).Should().BeFalse();
            _testInstance.TrySet(new[] { 1, 2, 3 }).Should().BeTrue();

            _testInstance.TrySet(JToken.Parse(@"{ ""value"": 1 }")).Should().BeFalse();
            _testInstance.TrySet(JToken.Parse(@"{ ""min"": 1, ""max"": 2 }")).Should().BeFalse();
            _testInstance.TrySet(JToken.Parse(@"{ ""values"": [ 1, 2, 3 ] }")).Should().BeTrue();

            _testInstance.NeedsToBeResolved.Should().BeTrue();
        }

        [Fact]
        public void Shoud_Fail_When_Values_Dont_Have_Correct_Type()
        {
            _testInstance.Comparison();

            _testInstance.TrySet("abc").Should().BeFalse();
            _testInstance.TrySet("abc", "def").Should().BeFalse();
            Action func = () => _testInstance.TrySet(JToken.Parse(@"{ ""value"": ""abc"" }"));
            func.Should().Throw<FormatException>();
        }

        private sealed class TestFilterSelector : FilterSelector<GenericSource, int>
        {
            internal TestFilterSelector(Expression<Func<GenericSource, int>> selector) : base(selector) {}

            [NotNull]
            public ComparisonResolver<GenericSource, int> Comparison()
            {
                var resolver = new ComparisonResolver<GenericSource, int>(new TestComparer(), Selector);
                FilterResolver = resolver;
                return resolver;
            }

            [NotNull]
            public RangeResolver<GenericSource, int> Range()
            {
                var resolver = new RangeResolver<GenericSource, int>(Selector, int.MinValue, int.MaxValue);
                FilterResolver = resolver;
                return resolver;
            }

            [NotNull]
            public ListResolver<GenericSource, int> List()
            {
                var resolver = new ListResolver<GenericSource, int>(Selector);
                FilterResolver = resolver;
                return resolver;
            }
        }

        private sealed class TestComparer : Comparer<GenericSource, int>
        {
            public override string FilterType { get; } = "TestComparer";

            [NotNull]
            public override Expression<Func<GenericSource, bool>> FilterExpression(Expression<Func<GenericSource, int>> selector, int selectedValue)
            {
                var valueConstant = Expression.Constant(selectedValue);
                var expression = Expression.Equal(selector.Body, valueConstant);
                return Expression.Lambda<Func<GenericSource, bool>>(expression, selector.Parameters);
            }
        }
    }
}
