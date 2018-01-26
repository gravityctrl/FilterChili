using System;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using GravityCTRL.FilterChili.Comparison;
using GravityCTRL.FilterChili.Exceptions;
using GravityCTRL.FilterChili.Providers;
using GravityCTRL.FilterChili.Resolvers;
using GravityCTRL.FilterChili.Resolvers.Comparison;
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
            _testInstance = new TestFilterSelector(new TestDomainProvider(source => source.Int));
        }

        [Fact]
        public void Should_Throw_Exception_If_No_Resolver_Was_Specified()
        {
            var queryable = new GenericSource[0].AsQueryable();

            Action domainAction = () => _testInstance.Domain();
            domainAction.ShouldThrow<MissingResolverException>();

            Action applyFilterAction = () => _testInstance.ApplyFilter(queryable);
            applyFilterAction.ShouldThrow<MissingResolverException>();

            Action setAvailableEntitiesAction = () => _testInstance.SetAvailableEntities(queryable).Wait();
            setAvailableEntitiesAction.ShouldThrow<MissingResolverException>();

            Action setSelectableEntitiesAction = () => _testInstance.SetSelectableEntities(queryable).Wait();
            setSelectableEntitiesAction.ShouldThrow<MissingResolverException>();

            Action needsToBeResolvedAction = () => _testInstance.NeedsToBeResolved = true;
            needsToBeResolvedAction.ShouldThrow<MissingResolverException>();
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
            _testInstance.With(domain => domain.Comparison("TestName"));

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
            _testInstance.With(domain => domain.Comparison("TestName")).Should().BeOfType<TestComparisonResolver>();
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
            _testInstance.With(domain => domain.Range("TestName")).Should().BeOfType<TestRangeResolver>();
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
            _testInstance.With(domain => domain.List("TestName")).Should().BeOfType<TestListResolver>();
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
            _testInstance.With(domain => domain.Comparison("TestName"));

            _testInstance.TrySet("abc").Should().BeFalse();
            _testInstance.TrySet("abc", "def").Should().BeFalse();
            Action func = () => _testInstance.TrySet(JToken.Parse(@"{ ""value"": ""abc"" }"));
            func.ShouldThrow<FormatException>();
        }

        

        private class TestFilterSelector : FilterSelector<GenericSource, int, TestDomainProvider>
        {
            internal TestFilterSelector(TestDomainProvider domainProvider) : base(domainProvider) {}
        }

        private class TestDomainProvider : DomainProvider<GenericSource, int>
        {
            internal TestDomainProvider(Expression<Func<GenericSource, int>> selector) : base(selector) {}

            public TestComparisonResolver Comparison(string name)
            {
                return new TestComparisonResolver(name, new TestComparer(), Selector);
            }

            public TestRangeResolver Range(string name)
            {
                return new TestRangeResolver(name, Selector);
            }

            public TestListResolver List(string name)
            {
                return new TestListResolver(name, Selector);
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
