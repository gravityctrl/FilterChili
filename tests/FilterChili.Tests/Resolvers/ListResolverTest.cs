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
using System.Threading.Tasks;
using FluentAssertions;
using GravityCTRL.FilterChili.Expressions;
using GravityCTRL.FilterChili.Models;
using GravityCTRL.FilterChili.Resolvers;
using GravityCTRL.FilterChili.Tests.TestSupport.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace GravityCTRL.FilterChili.Tests.Resolvers
{
    public class ListResolverTest
    {
        private const string TEST_NAME = "TestName";

        private readonly ListResolver<GenericSource, int> _instance;

        public ListResolverTest()
        {
            _instance = new TestListResolver(TEST_NAME, source => source.Int);
        }

        [Fact]
        public void Should_Initialize_Instance_Correctly()
        {
            _instance.FilterType.Should().Be("List");
            _instance.SourceType.Should().Be("GenericSource");
            _instance.TargetType.Should().Be("Int32");

            _instance.NeedsToBeResolved.Should().BeTrue();
            _instance.SelectedValues.Should().BeEmpty();
            _instance.Values.Should().BeEmpty();
        }

        [Fact]
        public void Should_Set_Values_Correctly_With_Params()
        {
            _instance.NeedsToBeResolved = false;
            _instance.Set(-1, 1);

            _instance.SelectedValues.Should().Contain(new[] {-1, 1});
            _instance.NeedsToBeResolved.Should().Be(true);
        }

        [Fact]
        public void Should_Set_Values_Correctly_Without_Params()
        {
            _instance.NeedsToBeResolved = false;
            _instance.Set(new List<int> { -1, 1 });

            _instance.SelectedValues.Should().Contain(new[] { -1, 1 });
            _instance.NeedsToBeResolved.Should().Be(true);
        }

        [Fact]
        public void Should_Set_Value_With_TrySet_If_JToken_Is_Correct()
        {
            _instance.NeedsToBeResolved = false;
            _instance.TrySet(JToken.Parse(@"{ ""values"": [ -2, 2 ] }"));

            _instance.SelectedValues.Should().Contain(new[] { -2, 2 });
            _instance.NeedsToBeResolved.Should().Be(true);
        }

        [Fact]
        public void Should_Set_Value_With_TrySet_If_JToken_Value_Can_Be_Interpreted()
        {
            _instance.NeedsToBeResolved = false;
            _instance.TrySet(JToken.Parse(@"{ ""values"": [ ""-3"", ""3"" ] }"));

            _instance.SelectedValues.Should().Contain(new[] { -3, 3 });
            _instance.NeedsToBeResolved.Should().Be(true);
        }

        [Fact]
        public void Should_Not_Set_Value_With_TrySet_If_JToken_Value_Cannot_Be_Interpreted()
        {
            _instance.NeedsToBeResolved = false;
            Action func = () => _instance.TrySet(JToken.Parse(@"{ ""values"": [ ""-3a"", ""3a"" ] }"));

            func.ShouldThrow<FormatException>();
            _instance.SelectedValues.Should().BeEmpty();
            _instance.NeedsToBeResolved.Should().Be(false);
        }

        [Fact]
        public void Should_Not_Set_Value_With_TrySet_If_JToken_Has_Invalid_Content()
        {
            _instance.NeedsToBeResolved = false;
            _instance.TrySet(JToken.Parse(@"{ ""value"": [ -4, 4 ] }"));

            _instance.SelectedValues.Should().BeEmpty();
            _instance.NeedsToBeResolved.Should().Be(false);
        }

        [Fact]
        public async Task Should_Return_Null_For_SetAvailableEntities_If_Queryable_Is_Empty()
        {
            _instance.NeedsToBeResolved = false;
            await _instance.SetAvailableEntities(new GenericSource[0].AsQueryable());

            _instance.Values.Should().BeEmpty();
        }

        [Fact]
        public async Task Should_Return_Null_For_SetSelectableEntities_If_Queryable_Is_Empty()
        {
            _instance.NeedsToBeResolved = false;
            await _instance.SetSelectableEntities(new GenericSource[0].AsQueryable());

            _instance.Values.Should().BeEmpty();
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

            await _instance.SetAvailableEntities(new AsyncEnumerable<GenericSource>(items));
            await _instance.SetSelectableEntities(items.AsQueryable().Skip(1).Take(3));

            var expected = new[]
            {
                new Item<int>
                {
                    CanBeSelected = false,
                    IsSelected = false,
                    Value = -2
                },
                new Item<int>
                {
                    CanBeSelected = true,
                    IsSelected = false,
                    Value = -1
                },
                new Item<int>
                {
                    CanBeSelected = true,
                    IsSelected = false,
                    Value = 0
                },
                new Item<int>
                {
                    CanBeSelected = true,
                    IsSelected = false,
                    Value = 1
                },
                new Item<int>
                {
                    CanBeSelected = false,
                    IsSelected = false,
                    Value = 2
                }
            };

            var expectedJson = JsonConvert.SerializeObject(expected);
            JsonConvert.SerializeObject(_instance.Values).Should().Be(expectedJson);

            await _instance.SetAvailableEntities(new AsyncEnumerable<GenericSource>(new List<GenericSource>()));

            _instance.Values.Should().BeEmpty();
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
            await _instance.SetSelectableEntities(items.AsQueryable().Skip(1).Take(3));

            var expected = new[]
            {
                new Item<int>
                {
                    CanBeSelected = false,
                    IsSelected = false,
                    Value = -2
                },
                new Item<int>
                {
                    CanBeSelected = true,
                    IsSelected = false,
                    Value = -1
                },
                new Item<int>
                {
                    CanBeSelected = true,
                    IsSelected = false,
                    Value = 0
                },
                new Item<int>
                {
                    CanBeSelected = true,
                    IsSelected = false,
                    Value = 1
                },
                new Item<int>
                {
                    CanBeSelected = false,
                    IsSelected = false,
                    Value = 2
                }
            };

            var expectedJson = JsonConvert.SerializeObject(expected);
            JsonConvert.SerializeObject(_instance.Values).Should().Be(expectedJson);
            _instance.NeedsToBeResolved.Should().Be(false);
        }

        [Fact]
        public async Task Should_Not_Set_CanBeSelected_If_There_Are_No_Selectable_Values()
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
            _instance.Set(-1, 2);

            var expected = new[]
            {
                new Item<int>
                {
                    CanBeSelected = false,
                    IsSelected = false,
                    Value = -2
                },
                new Item<int>
                {
                    CanBeSelected = false,
                    IsSelected = true,
                    Value = -1
                },
                new Item<int>
                {
                    CanBeSelected = false,
                    IsSelected = false,
                    Value = 0
                },
                new Item<int>
                {
                    CanBeSelected = false,
                    IsSelected = false,
                    Value = 1
                },
                new Item<int>
                {
                    CanBeSelected = false,
                    IsSelected = true,
                    Value = 2
                }
            };

            var expectedJson = JsonConvert.SerializeObject(expected);
            JsonConvert.SerializeObject(_instance.Values).Should().Be(expectedJson);
            _instance.NeedsToBeResolved.Should().Be(true);
        }

        [Fact]
        public void Should_Get_Values_If_There_Are_Only_Selected_Ones()
        {
            _instance.NeedsToBeResolved = false;
            _instance.Set(-1, 2);

            var expected = new[]
            {
                new Item<int>
                {
                    CanBeSelected = false,
                    IsSelected = true,
                    Value = -1
                },
                new Item<int>
                {
                    CanBeSelected = false,
                    IsSelected = true,
                    Value = 2
                }
            };

            var expectedJson = JsonConvert.SerializeObject(expected);
            JsonConvert.SerializeObject(_instance.Values).Should().Be(expectedJson);
            _instance.NeedsToBeResolved.Should().Be(true);
        }

        private class TestListResolver : ListResolver<GenericSource, int>
        {
            internal TestListResolver(string name, Expression<Func<GenericSource, int>> selector) : base(name, selector) {}

            protected override Expression<Func<GenericSource, bool>> FilterExpression()
            {
                var selectedValueExpressions = SelectedValues.Select(value => Expression.Constant(value));
                var equalsExpressions = selectedValueExpressions.Select(expression => Expression.Equal(expression, Selector.Body));
                var orExpression = equalsExpressions.Or();
                return orExpression == null ? null : Expression.Lambda<Func<GenericSource, bool>>(orExpression, Selector.Parameters);
            }
        }
    }
}
