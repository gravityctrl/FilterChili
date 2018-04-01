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
using System.Threading.Tasks;
using FluentAssertions;
using GravityCTRL.FilterChili.Models;
using GravityCTRL.FilterChili.Tests.TestSupport.Models;
using Newtonsoft.Json.Linq;
using Xunit;

namespace GravityCTRL.FilterChili.Tests.Resolvers
{
    public sealed class RangeResolverTest
    {
        private readonly RangeResolver<GenericSource, int> _testInstance;

        public RangeResolverTest()
        {
            _testInstance = new RangeResolver<GenericSource, int>(source => source.Int, -5, 5);
        }

        [Fact]
        public void Should_Initialize_Instance_Correctly()
        {
            _testInstance.FilterType.Should().Be("Range");
            _testInstance.SourceType.Should().Be("GenericSource");
            _testInstance.TargetType.Should().Be("Int32");

            _testInstance.NeedsToBeResolved.Should().BeTrue();
            _testInstance.SelectableRange.Should().BeNull();
            _testInstance.SelectedRange.Min.Should().Be(-5);
            _testInstance.SelectedRange.Max.Should().Be(5);
            _testInstance.TotalRange.Should().BeNull();
        }

        [Fact]
        public void Should_Set_Value_Correctly()
        {
            _testInstance.NeedsToBeResolved = false;
            _testInstance.Set(-1, 1);

            _testInstance.SelectedRange.Min.Should().Be(-1);
            _testInstance.SelectedRange.Max.Should().Be(1);
            _testInstance.NeedsToBeResolved.Should().Be(true);
        }

        [Fact]
        public void Should_Set_Value_With_TrySet_If_JToken_Is_Correct()
        {
            _testInstance.NeedsToBeResolved = false;
            _testInstance.TrySet(JToken.Parse(@"{ ""min"": -2, ""max"": 2 }"));

            _testInstance.SelectedRange.Min.Should().Be(-2);
            _testInstance.SelectedRange.Max.Should().Be(2);
            _testInstance.NeedsToBeResolved.Should().Be(true);
        }

        [Fact]
        public void Should_Set_Value_With_TrySet_If_JToken_Value_Can_Be_Interpreted()
        {
            _testInstance.NeedsToBeResolved = false;
            _testInstance.TrySet(JToken.Parse(@"{ ""min"": ""-3"", ""max"": ""3"" }"));

            _testInstance.SelectedRange.Min.Should().Be(-3);
            _testInstance.SelectedRange.Max.Should().Be(3);
            _testInstance.NeedsToBeResolved.Should().Be(true);
        }

        [Fact]
        public void Should_Not_Be_Able_To_Set_Null_JToken_As_Filter()
        {
            _testInstance.TrySet(null).Should().BeFalse();
        }

        [Fact]
        public void Should_Not_Set_Value_With_TrySet_If_JToken_Value_Cannot_Be_Interpreted()
        {
            _testInstance.NeedsToBeResolved = false;
            Action func = () => _testInstance.TrySet(JToken.Parse(@"{ ""min"": ""-4a"", ""max"": ""4a"" }"));

            func.Should().Throw<FormatException>();
            _testInstance.SelectedRange.Min.Should().Be(-5);
            _testInstance.SelectedRange.Max.Should().Be(5);
            _testInstance.NeedsToBeResolved.Should().Be(false);
        }

        [Fact]
        public void Should_Not_Set_Value_With_TrySet_If_JToken_Has_Invalid_Content()
        {
            _testInstance.NeedsToBeResolved = false;
            _testInstance.TrySet(JToken.Parse(@"{ ""values"": 4 }"));

            _testInstance.SelectedRange.Min.Should().Be(-5);
            _testInstance.SelectedRange.Max.Should().Be(5);
            _testInstance.NeedsToBeResolved.Should().Be(false);
        }

        [Fact]
        public async Task Should_Return_Null_For_SetAvailableEntities_If_Queryable_Is_Empty()
        {
            _testInstance.NeedsToBeResolved = false;
            await _testInstance.SetEntities(Option.Some(new GenericSource[0].AsQueryable()), Option.None<IQueryable<GenericSource>>());

            _testInstance.TotalRange.Should().BeNull();
        }

        [Fact]
        public async Task Should_Return_Null_For_SetSelectableEntities_If_Queryable_Is_Empty()
        {
            _testInstance.NeedsToBeResolved = false;
            await _testInstance.SetEntities(Option.None<IQueryable<GenericSource>>(), Option.Some(new GenericSource[0].AsQueryable()));

            _testInstance.SelectableRange.Should().BeNull();
        }

        [Fact]
        public async Task Should_Support_Async_Queryables()
        {
            var items = new[]
            {
                new GenericSource { Int = -2 },
                new GenericSource { Int = -1 },
                new GenericSource { Int = 0 },
                new GenericSource { Int = 1 },
                new GenericSource { Int = 2 }
            };

            await _testInstance.SetEntities(Option.Some(new AsyncEnumerable<GenericSource>(items).AsQueryable()), Option.None<IQueryable<GenericSource>>());

            _testInstance.TotalRange.Min.Should().Be(-2);
            _testInstance.TotalRange.Max.Should().Be(2);

            await _testInstance.SetEntities(Option.Some(new AsyncEnumerable<GenericSource>(new List<GenericSource>()).AsQueryable()), Option.None<IQueryable<GenericSource>>());

            _testInstance.TotalRange.Should().BeNull();
        }

        [Fact]
        public async Task Should_Set_TotalRange_On_Calling_SetAvailableEntities()
        {
            _testInstance.NeedsToBeResolved = false;
            var items = new[]
            {
                new GenericSource { Int = -2 },
                new GenericSource { Int = -1 },
                new GenericSource { Int = 0 },
                new GenericSource { Int = 1 },
                new GenericSource { Int = 2 }
            };

            await _testInstance.SetEntities(Option.Some(items.AsQueryable()), Option.None<IQueryable<GenericSource>>());

            _testInstance.TotalRange.Min.Should().Be(-2);
            _testInstance.TotalRange.Max.Should().Be(2);
            _testInstance.SelectedRange.Min.Should().Be(-5);
            _testInstance.SelectedRange.Max.Should().Be(5);
            _testInstance.NeedsToBeResolved.Should().Be(false);
        }

        [Fact]
        public async Task Should_Set_SelectableRange_On_Calling_SetSelectableEntities()
        {
            _testInstance.NeedsToBeResolved = false;
            var items = new[]
            {
                new GenericSource { Int = -1 },
                new GenericSource { Int = 0 },
                new GenericSource { Int = 1 }
            };

            await _testInstance.SetEntities(Option.None<IQueryable<GenericSource>>(), Option.Some(items.AsQueryable()));

            _testInstance.SelectableRange.Min.Should().Be(-1);
            _testInstance.SelectableRange.Max.Should().Be(1);
            _testInstance.SelectedRange.Min.Should().Be(-5);
            _testInstance.SelectedRange.Max.Should().Be(5);
            _testInstance.NeedsToBeResolved.Should().Be(false);
        }

        [Fact]
        public void Should_Return_Correct_Amount_Of_Values_When_Executing_Filter()
        {
            _testInstance.NeedsToBeResolved = false;
            var items = new[]
            {
                new GenericSource { Int = -2 },
                new GenericSource { Int = -1 },
                new GenericSource { Int = 0 },
                new GenericSource { Int = 1 },
                new GenericSource { Int = 2 }
            };

            _testInstance.Set(-1, 1);
            _testInstance.NeedsToBeResolved.Should().Be(true);

            var result1 = _testInstance.ExecuteFilter(items.AsQueryable());
            result1.Should().HaveCount(3);

            var expectedItems = items.Skip(1).Take(3).ToArray();
            result1.Should().Contain(expectedItems);

            var result2 = _testInstance.ExecuteFilter(new GenericSource[0].AsQueryable());
            result2.Should().HaveCount(0);
        }

        [Fact]
        public void Should_Only_Compare_Min_If_Value_Isnt_Outside_Of_Range_Borders()
        {
            _testInstance.NeedsToBeResolved = false;
            var items = new[]
            {
                new GenericSource { Int = -2 },
                new GenericSource { Int = -1 },
                new GenericSource { Int = 0 },
                new GenericSource { Int = 1 },
                new GenericSource { Int = 2 }
            };

            _testInstance.Set(-6, 1);
            _testInstance.NeedsToBeResolved.Should().Be(true);

            var result = _testInstance.ExecuteFilter(items.AsQueryable());
            result.Should().HaveCount(4);

            var expectedItems = items.Take(4).ToArray();
            result.Should().Contain(expectedItems);
        }

        [Fact]
        public void Should_Only_Compare_Max_If_Value_Isnt_Outside_Of_Range_Borders()
        {
            _testInstance.NeedsToBeResolved = false;
            var items = new[]
            {
                new GenericSource { Int = -2 },
                new GenericSource { Int = -1 },
                new GenericSource { Int = 0 },
                new GenericSource { Int = 1 },
                new GenericSource { Int = 2 }
            };

            _testInstance.Set(-1, 6);
            _testInstance.NeedsToBeResolved.Should().Be(true);

            var result = _testInstance.ExecuteFilter(items.AsQueryable());
            result.Should().HaveCount(4);

            var expectedItems = items.Skip(1).Take(4).ToArray();
            result.Should().Contain(expectedItems);
        }

        [Fact]
        public void Should_Only_Compare_Min_And_Max_If_Values_Arent_Outside_Of_Range_Borders()
        {
            _testInstance.NeedsToBeResolved = false;
            var items = new[]
            {
                new GenericSource { Int = -2 },
                new GenericSource { Int = -1 },
                new GenericSource { Int = 0 },
                new GenericSource { Int = 1 },
                new GenericSource { Int = 2 }
            };

            _testInstance.Set(-6, 6);
            _testInstance.NeedsToBeResolved.Should().Be(true);

            var result = _testInstance.ExecuteFilter(items.AsQueryable());
            result.Should().HaveCount(5);

            var expectedItems = items.ToArray();
            result.Should().Contain(expectedItems);
        }
    }
}
