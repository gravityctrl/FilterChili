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
using GravityCTRL.FilterChili.Resolvers;
using GravityCTRL.FilterChili.Selectors;
using GravityCTRL.FilterChili.Tests.TestSupport.Models;
using Newtonsoft.Json.Linq;
using Xunit;

namespace GravityCTRL.FilterChili.Tests.Selectors
{
    public class FilterSelectorTest
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
            domainAction.ShouldThrow<MissingResolverException>().WithMessage(nameof(TestFilterSelector));

            Action applyFilterAction = () => _testInstance.ApplyFilter(queryable);
            applyFilterAction.ShouldThrow<MissingResolverException>().WithMessage(nameof(TestFilterSelector));

            Action setAvailableEntitiesAction = () => _testInstance.SetAvailableEntities(queryable).Wait();
            setAvailableEntitiesAction.ShouldThrow<MissingResolverException>().WithMessage(nameof(TestFilterSelector));

            Action setSelectableEntitiesAction = () => _testInstance.SetSelectableEntities(queryable).Wait();
            setSelectableEntitiesAction.ShouldThrow<MissingResolverException>().WithMessage(nameof(TestFilterSelector));

            Action needsToBeResolvedAction = () => _testInstance.NeedsToBeResolved = true;
            needsToBeResolvedAction.ShouldThrow<MissingResolverException>().WithMessage(nameof(TestFilterSelector));
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
            _testInstance.Comparison("TestName");

            var queryable = new GenericSource[0].AsQueryable();

            Action domainAction = () => _testInstance.Domain();
            domainAction.ShouldNotThrow<MissingResolverException>();

            Action applyFilterAction = () => _testInstance.ApplyFilter(queryable);
            applyFilterAction.ShouldNotThrow<MissingResolverException>();

            Action setAvailableEntitiesAction = () => _testInstance.SetAvailableEntities(queryable).Wait();
            setAvailableEntitiesAction.ShouldNotThrow<MissingResolverException>();

            Action setSelectableEntitiesAction = () => _testInstance.SetSelectableEntities(queryable).Wait();
            setSelectableEntitiesAction.ShouldNotThrow<MissingResolverException>();

            Action needsToBeResolvedAction = () => _testInstance.NeedsToBeResolved = true;
            needsToBeResolvedAction.ShouldNotThrow<MissingResolverException>();
        }

        [Fact]
        public void ComparisonResolver_Should_Be_Filled_With_Correct_TrySet_Method_Calls()
        {
            _testInstance.Comparison("TestName").Should().BeOfType<TestComparisonResolver>();
            _testInstance.Domain().Should().BeOfType<TestComparisonResolver>();
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
            _testInstance.Range("TestName").Should().BeOfType<TestRangeResolver>();
            _testInstance.Domain().Should().BeOfType<TestRangeResolver>();
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
            _testInstance.List("TestName").Should().BeOfType<TestListResolver>();
            _testInstance.Domain().Should().BeOfType<TestListResolver>();
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
            _testInstance.Comparison("TestName");

            _testInstance.TrySet("abc").Should().BeFalse();
            _testInstance.TrySet("abc", "def").Should().BeFalse();
            Action func = () => _testInstance.TrySet(JToken.Parse(@"{ ""value"": ""abc"" }"));
            func.ShouldThrow<FormatException>();
        }

        private class TestFilterSelector : FilterSelector<GenericSource, int>
        {
            internal TestFilterSelector(Expression<Func<GenericSource, int>> selector) : base(selector) {}

            public TestComparisonResolver Comparison(string name)
            {
                var resolver = new TestComparisonResolver(name, new TestComparer(), Selector);
                DomainResolver = resolver;
                return resolver;
            }

            public TestRangeResolver Range(string name)
            {
                var resolver = new TestRangeResolver(name, Selector);
                DomainResolver = resolver;
                return resolver;
            }

            public TestListResolver List(string name)
            {
                var resolver = new TestListResolver(name, Selector);
                DomainResolver = resolver;
                return resolver;
            }
        }

        private class TestComparisonResolver : ComparisonResolver<GenericSource, int>
        {
            internal TestComparisonResolver(string name, Comparer<GenericSource, int> comparer, Expression<Func<GenericSource, int>> selector) : base(name, comparer, selector) {}
        }

        private class TestRangeResolver : RangeResolver<GenericSource, int>
        {
            internal TestRangeResolver(string name, Expression<Func<GenericSource, int>> selector) : base(name, selector, int.MinValue, int.MaxValue) {}
        }

        private class TestListResolver : ListResolver<GenericSource, int>
        {
            internal TestListResolver(string name, Expression<Func<GenericSource, int>> selector) : base(name, selector) {}

            protected override Expression<Func<GenericSource, bool>> FilterExpression()
            {
                return null;
            }
        }

        private class TestComparer : Comparer<GenericSource, int>
        {
            public override string FilterType { get; } = "TestComparer";

            public override Expression<Func<GenericSource, bool>> FilterExpression(Expression<Func<GenericSource, int>> selector, int selectedValue)
            {
                var valueConstant = Expression.Constant(selectedValue);
                var expression = Expression.Equal(selector.Body, valueConstant);
                return Expression.Lambda<Func<GenericSource, bool>>(expression, selector.Parameters);
            }
        }
    }
}
