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
using System.Threading.Tasks;
using FluentAssertions;
using GravityCTRL.FilterChili.Resolvers;
using GravityCTRL.FilterChili.Tests.Models;
using Newtonsoft.Json.Linq;
using Xunit;

namespace GravityCTRL.FilterChili.Tests.Resolvers
{
    public class RangeResolverTest
    {
        private const string TEST_NAME = "TestName";

        private readonly RangeResolver<GenericSource, int> _instance;

        public RangeResolverTest()
        {
            _instance = new TestRangeResolver(TEST_NAME, source => source.Int);
        }

        [Fact]
        public void Should_Initialize_Instance_Correctly()
        {
            _instance.FilterType.Should().Be("Range");
            _instance.SourceType.Should().Be("GenericSource");
            _instance.TargetType.Should().Be("Int32");

            _instance.NeedsToBeResolved.Should().BeTrue();
            _instance.SelectableRange.Should().BeNull();
            _instance.SelectedRange.Min.Should().Be(-5);
            _instance.SelectedRange.Max.Should().Be(5);
            _instance.TotalRange.Should().BeNull();
        }

        [Fact]
        public void Should_Set_Value_Correctly()
        {
            _instance.NeedsToBeResolved = false;
            _instance.Set(-1, 1);

            _instance.SelectedRange.Min.Should().Be(-1);
            _instance.SelectedRange.Max.Should().Be(1);
            _instance.NeedsToBeResolved.Should().Be(true);
        }

        [Fact]
        public void Should_Set_Value_With_TrySet_If_JToken_Is_Correct()
        {
            _instance.NeedsToBeResolved = false;
            _instance.TrySet(JToken.Parse(@"{ ""min"": -2, ""max"": 2 }"));

            _instance.SelectedRange.Min.Should().Be(-2);
            _instance.SelectedRange.Max.Should().Be(2);
            _instance.NeedsToBeResolved.Should().Be(true);
        }

        [Fact]
        public void Should_Set_Value_With_TrySet_If_JToken_Value_Can_Be_Interpreted()
        {
            _instance.NeedsToBeResolved = false;
            _instance.TrySet(JToken.Parse(@"{ ""min"": ""-3"", ""max"": ""3"" }"));

            _instance.SelectedRange.Min.Should().Be(-3);
            _instance.SelectedRange.Max.Should().Be(3);
            _instance.NeedsToBeResolved.Should().Be(true);
        }

        [Fact]
        public void Should_Not_Set_Value_With_TrySet_If_JToken_Value_Cannot_Be_Interpreted()
        {
            _instance.NeedsToBeResolved = false;
            Action func = () => _instance.TrySet(JToken.Parse(@"{ ""min"": ""-4a"", ""max"": ""4a"" }"));

            func.ShouldThrow<FormatException>();
            _instance.SelectedRange.Min.Should().Be(-5);
            _instance.SelectedRange.Max.Should().Be(5);
            _instance.NeedsToBeResolved.Should().Be(false);
        }

        [Fact]
        public void Should_Not_Set_Value_With_TrySet_If_JToken_Has_Invalid_Content()
        {
            _instance.NeedsToBeResolved = false;
            _instance.TrySet(JToken.Parse(@"{ ""values"": 4 }"));

            _instance.SelectedRange.Min.Should().Be(-5);
            _instance.SelectedRange.Max.Should().Be(5);
            _instance.NeedsToBeResolved.Should().Be(false);
        }

        [Fact]
        public async Task Should_Return_Null_For_SetAvailableEntities_If_Queryable_Is_Empty()
        {
            _instance.NeedsToBeResolved = false;
            await _instance.SetAvailableEntities(new GenericSource[0].AsQueryable());

            _instance.TotalRange.Should().BeNull();
        }

        [Fact]
        public async Task Should_Return_Null_For_SetSelectableEntities_If_Queryable_Is_Empty()
        {
            _instance.NeedsToBeResolved = false;
            await _instance.SetSelectableEntities(new GenericSource[0].AsQueryable());

            _instance.SelectableRange.Should().BeNull();
        }

        [Fact]
        public async Task Should_Set_TotalRange_On_Calling_SetAvailableEntities()
        {
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
            _instance.SelectedRange.Min.Should().Be(-5);
            _instance.SelectedRange.Max.Should().Be(5);
            _instance.NeedsToBeResolved.Should().Be(false);
        }

        [Fact]
        public async Task Should_Set_SelectableRange_On_Calling_SetSelectableEntities()
        {
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
            _instance.SelectedRange.Min.Should().Be(-5);
            _instance.SelectedRange.Max.Should().Be(5);
            _instance.NeedsToBeResolved.Should().Be(false);
        }

        [Fact]
        public void Should_Return_Correct_Amount_Of_Values_When_Executing_Filter()
        {
            _instance.NeedsToBeResolved = false;
            var items = new[]
            {
                new GenericSource { Int = -2 },
                new GenericSource { Int = -1 },
                new GenericSource { Int = 0 },
                new GenericSource { Int = 1 },
                new GenericSource { Int = 2 }
            };

            _instance.Set(-1, 1);
            _instance.NeedsToBeResolved.Should().Be(true);

            var result1 = _instance.ExecuteFilter(items.AsQueryable());
            result1.Should().HaveCount(3);

            var expectedItems = items.Skip(1).Take(3).ToArray();
            result1.Should().Contain(expectedItems);

            var result2 = _instance.ExecuteFilter(new GenericSource[0].AsQueryable());
            result2.Should().HaveCount(0);
        }

        [Fact]
        public void Should_Only_Compare_Min_If_Value_Isnt_Outside_Of_Range_Borders()
        {
            _instance.NeedsToBeResolved = false;
            var items = new[]
            {
                new GenericSource { Int = -2 },
                new GenericSource { Int = -1 },
                new GenericSource { Int = 0 },
                new GenericSource { Int = 1 },
                new GenericSource { Int = 2 }
            };

            _instance.Set(-6, 1);
            _instance.NeedsToBeResolved.Should().Be(true);

            var result = _instance.ExecuteFilter(items.AsQueryable());
            result.Should().HaveCount(4);

            var expectedItems = items.Take(4).ToArray();
            result.Should().Contain(expectedItems);
        }

        [Fact]
        public void Should_Only_Compare_Max_If_Value_Isnt_Outside_Of_Range_Borders()
        {
            _instance.NeedsToBeResolved = false;
            var items = new[]
            {
                new GenericSource { Int = -2 },
                new GenericSource { Int = -1 },
                new GenericSource { Int = 0 },
                new GenericSource { Int = 1 },
                new GenericSource { Int = 2 }
            };

            _instance.Set(-1, 6);
            _instance.NeedsToBeResolved.Should().Be(true);

            var result = _instance.ExecuteFilter(items.AsQueryable());
            result.Should().HaveCount(4);

            var expectedItems = items.Skip(1).Take(4).ToArray();
            result.Should().Contain(expectedItems);
        }

        [Fact]
        public void Should_Only_Compare_Min_And_Max_If_Values_Arent_Outside_Of_Range_Borders()
        {
            _instance.NeedsToBeResolved = false;
            var items = new[]
            {
                new GenericSource { Int = -2 },
                new GenericSource { Int = -1 },
                new GenericSource { Int = 0 },
                new GenericSource { Int = 1 },
                new GenericSource { Int = 2 }
            };

            _instance.Set(-6, 6);
            _instance.NeedsToBeResolved.Should().Be(true);

            var result = _instance.ExecuteFilter(items.AsQueryable());
            result.Should().HaveCount(5);

            var expectedItems = items.ToArray();
            result.Should().Contain(expectedItems);
        }

        private class TestRangeResolver : RangeResolver<GenericSource, int>
        {
            internal TestRangeResolver(string name, Expression<Func<GenericSource, int>> selector) : base(name, selector, -5, 5) {}
        }
    }
}
