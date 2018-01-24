using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using GravityCTRL.FilterChili.Comparison;
using GravityCTRL.FilterChili.Resolvers;
using GravityCTRL.FilterChili.Tests.Models;
using Newtonsoft.Json.Linq;
using Xunit;

namespace GravityCTRL.FilterChili.Tests.Resolvers
{
    public class ComparisonResolverTest
    {
        private const string TEST_NAME = "TestName";

        private readonly ComparisonResolver<GenericSource, int> _instance;

        public ComparisonResolverTest()
        {
            _instance = new TestComparisonResolver<int>(TEST_NAME, new TestComparer(), source => source.Int);
        }

        [Fact]
        public void Should_Initialize_Instance_Correctly()
        {
            _instance.FilterType.Should().Be("TestComparer");
            _instance.NeedsToBeResolved.Should().BeTrue();

            _instance.SelectableRange.Should().BeNull();
            _instance.SelectedValue.Should().Be(0);
            _instance.TotalRange.Should().BeNull();
        }

        [Fact]
        public void Should_Set_Value_Correctly()
        {
            _instance.FilterType.Should().Be("TestComparer");
            _instance.NeedsToBeResolved = false;
            _instance.Set(1);

            _instance.SelectedValue.Should().Be(1);
            _instance.NeedsToBeResolved.Should().Be(true);
        }

        [Fact]
        public void Should_Set_Value_With_TrySet_If_JToken_Is_Correct()
        {
            _instance.FilterType.Should().Be("TestComparer");
            _instance.NeedsToBeResolved = false;
            _instance.TrySet(JToken.Parse(@"{ ""value"": 2 }"));

            _instance.SelectedValue.Should().Be(2);
            _instance.NeedsToBeResolved.Should().Be(true);
        }

        [Fact]
        public void Should_Set_Value_With_TrySet_If_JToken_Value_Can_Be_Interpreted()
        {
            _instance.FilterType.Should().Be("TestComparer");
            _instance.NeedsToBeResolved = false;
            _instance.TrySet(JToken.Parse(@"{ ""value"": ""3"" }"));

            _instance.SelectedValue.Should().Be(3);
            _instance.NeedsToBeResolved.Should().Be(true);
        }

        [Fact]
        public void Should_Not_Set_Value_With_TrySet_If_JToken_Value_Cannot_Be_Interpreted()
        {
            _instance.FilterType.Should().Be("TestComparer");
            _instance.NeedsToBeResolved = false;
            Action func = () => _instance.TrySet(JToken.Parse(@"{ ""value"": ""3a"" }"));

            func.ShouldThrow<FormatException>();
            _instance.SelectedValue.Should().Be(0);
            _instance.NeedsToBeResolved.Should().Be(false);
        }

        [Fact]
        public void Should_Not_Set_Value_With_TrySet_If_JToken_Has_Invalid_Content()
        {
            _instance.FilterType.Should().Be("TestComparer");
            _instance.NeedsToBeResolved = false;
            _instance.TrySet(JToken.Parse(@"{ ""values"": 4 }"));

            _instance.SelectedValue.Should().Be(0);
            _instance.NeedsToBeResolved.Should().Be(false);
        }

        [Fact]
        public async Task Should_Return_Null_For_SetAvailableEntities_If_Queryable_Is_Empty()
        {
            _instance.FilterType.Should().Be("TestComparer");
            _instance.NeedsToBeResolved = false;
            await _instance.SetAvailableEntities(new GenericSource[0].AsQueryable());

            _instance.TotalRange.Should().BeNull();
        }

        [Fact]
        public async Task Should_Return_Null_For_SetSelectableEntities_If_Queryable_Is_Empty()
        {
            _instance.FilterType.Should().Be("TestComparer");
            _instance.NeedsToBeResolved = false;
            await _instance.SetSelectableEntities(new GenericSource[0].AsQueryable());

            _instance.SelectableRange.Should().BeNull();
        }

        [Fact]
        public async Task Should_Set_TotalRange_On_Calling_SetAvailableEntities()
        {
            _instance.FilterType.Should().Be("TestComparer");
            _instance.NeedsToBeResolved = false;
            var items = new[]
            {
                new GenericSource { Int = -2 },
                new GenericSource { Int = -1 },
                new GenericSource { Int = 0 },
                new GenericSource { Int = 1 },
                new GenericSource { Int = 2 }
            };

            await _instance.SetAvailableEntities(items.AsQueryable());

            _instance.TotalRange.Min.Should().Be(-2);
            _instance.TotalRange.Max.Should().Be(2);
            _instance.SelectedValue.Should().Be(0);
            _instance.NeedsToBeResolved.Should().Be(false);
        }

        [Fact]
        public async Task Should_Set_SelectableRange_On_Calling_SetSelectableEntities()
        {
            _instance.FilterType.Should().Be("TestComparer");
            _instance.NeedsToBeResolved = false;
            var items = new[]
            {
                new GenericSource { Int = -1 },
                new GenericSource { Int = 0 },
                new GenericSource { Int = 1 }
            };

            await _instance.SetSelectableEntities(items.AsQueryable());

            _instance.SelectableRange.Min.Should().Be(-1);
            _instance.SelectableRange.Max.Should().Be(1);
            _instance.SelectedValue.Should().Be(0);
            _instance.NeedsToBeResolved.Should().Be(false);
        }

        [Fact]
        public void Should_Return_Correct_Amount_Of_Values_When_Executing_Filter()
        {
            _instance.FilterType.Should().Be("TestComparer");
            _instance.NeedsToBeResolved = false;
            var items = new[]
            {
                new GenericSource { Int = -2 },
                new GenericSource { Int = -1 },
                new GenericSource { Int = 0 },
                new GenericSource { Int = 1 },
                new GenericSource { Int = 2 }
            };

            _instance.Set(1);
            _instance.NeedsToBeResolved.Should().Be(true);

            var result1 = _instance.ExecuteFilter(items.AsQueryable());
            result1.Should().HaveCount(1);

            var expectedItem = items[3];
            result1.First().Should().Be(expectedItem);

            var result2 = _instance.ExecuteFilter(new GenericSource[0].AsQueryable());
            result2.Should().HaveCount(0);
        }

        private class TestComparisonResolver<TSelector> : ComparisonResolver<GenericSource, TSelector> where TSelector : IComparable
        {
            internal TestComparisonResolver(string name, Comparer<GenericSource, TSelector> comparer, Expression<Func<GenericSource, TSelector>> selector) : base(name, comparer, selector) {}
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
