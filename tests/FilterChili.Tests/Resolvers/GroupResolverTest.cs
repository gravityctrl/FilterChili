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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace GravityCTRL.FilterChili.Tests.Resolvers
{
    public sealed class GroupResolverTest
    {
        private readonly GroupResolver<GenericSource, int, string> _testInstance;

        public GroupResolverTest()
        {
            _testInstance = new GroupResolver<GenericSource, int, string>(source => source.Int, source => source.String);
        }

        [Fact]
        public void Should_Initialize_Instance_Correctly()
        {
            _testInstance.FilterType.Should().Be("Group");
            _testInstance.SourceType.Should().Be("GenericSource");
            _testInstance.TargetType.Should().Be("Int32");

            _testInstance.NeedsToBeResolved.Should().BeTrue();
            _testInstance.Selection.TryGetLeft(out var left).Should().BeTrue();
            left.Should().BeEmpty();
            _testInstance.Groups.Should().BeEmpty();
        }

        [Fact]
        public void Should_Set_Values_Correctly_With_Params()
        {
            _testInstance.NeedsToBeResolved = false;
            _testInstance.Set(-1, 1);

            _testInstance.Selection.TryGetLeft(out var left).Should().BeTrue();
            left.Should().Contain(new[] { -1, 1 });

            _testInstance.NeedsToBeResolved.Should().BeTrue();
        }

        [Fact]
        public void Should_Set_Values_Correctly_Without_Params()
        {
            _testInstance.NeedsToBeResolved = false;
            _testInstance.Set(new List<int> { -1, 1 });

            _testInstance.Selection.TryGetLeft(out var left).Should().BeTrue();
            left.Should().Contain(new[] { -1, 1 });

            _testInstance.NeedsToBeResolved.Should().BeTrue();
        }

        [Fact]
        public void Should_Set_Groups_Correctly_With_Params()
        {
            _testInstance.NeedsToBeResolved = false;
            _testInstance.SetGroups("Category1", "Category2");

            _testInstance.Selection.TryGetRight(out var right).Should().BeTrue();
            right.Should().Contain(new[] { "Category1", "Category2" });

            _testInstance.NeedsToBeResolved.Should().BeTrue();
        }

        [Fact]
        public void Should_Set_Groups_Correctly_Without_Params()
        {
            _testInstance.NeedsToBeResolved = false;
            _testInstance.SetGroups(new List<string> { "Category1", "Category2" });

            _testInstance.Selection.TryGetRight(out var right).Should().BeTrue();
            right.Should().Contain(new[] { "Category1", "Category2" });

            _testInstance.NeedsToBeResolved.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Set_Selected_Values_With_SetGroups_Method()
        {
            _testInstance.NeedsToBeResolved = false;

            var items = new[]
            {
                new GenericSource { Int = -2, String = "Category1" },
                new GenericSource { Int = -1, String = "Category1" },
                new GenericSource { Int = 2, String = "Category1" },
                new GenericSource { Int = 0, String = "Category2" },
                new GenericSource { Int = 1, String = "Category2" },
                new GenericSource { Int = 2, String = "Category2" },
                new GenericSource { Int = 2, String = "Category2" },
                new GenericSource { Int = 3, String = "Category3" }
            };

            await _testInstance.SetEntities(Option.Some(new List<GenericSource>(items).AsQueryable()), Option.None<IQueryable<GenericSource>>());

            _testInstance.SetGroups("Category1", "Category3");

            _testInstance.Selection.TryGetRight(out var right).Should().BeTrue();
            right.Should().Contain(new[] { "Category1", "Category3" });

            _testInstance.NeedsToBeResolved.Should().BeTrue();

            _testInstance.SetGroups(new List<string> { "Category1", "Category3" });

            _testInstance.Selection.TryGetRight(out right).Should().BeTrue();
            right.Should().Contain(new[] { "Category1", "Category3" });

            _testInstance.NeedsToBeResolved.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Set_Selected_Values_With_TrySet_If_JToken_Is_Correct_Groups_Collection()
        {
            _testInstance.NeedsToBeResolved = false;

            var items = new[]
            {
                new GenericSource { Int = -2, String = "Category1" },
                new GenericSource { Int = -1, String = "Category1" },
                new GenericSource { Int = 2, String = "Category1" },
                new GenericSource { Int = 0, String = "Category2" },
                new GenericSource { Int = 1, String = "Category2" },
                new GenericSource { Int = 2, String = "Category2" },
                new GenericSource { Int = 2, String = "Category2" },
                new GenericSource { Int = 3, String = "Category3" }
            };

            await _testInstance.SetEntities(Option.Some(new List<GenericSource>(items).AsQueryable()), Option.None<IQueryable<GenericSource>>());

            _testInstance.TrySet(JToken.Parse(@"{ ""groups"": [ ""Category1"", ""Category3"" ] }"));

            _testInstance.Selection.TryGetRight(out var right).Should().BeTrue();
            right.Should().Contain(new[] { "Category1", "Category3" });

            _testInstance.NeedsToBeResolved.Should().BeTrue();
        }

        [Fact]
        public void Should_Set_Selected_Values_TrySet_If_JToken_Is_Correct_Values_Collection()
        {
            _testInstance.NeedsToBeResolved = false;
            _testInstance.TrySet(JToken.Parse(@"{ ""values"": [ -2, 2 ] }"));

            _testInstance.Selection.TryGetLeft(out var left).Should().BeTrue();
            left.Should().Contain(new[] { -2, 2 });

            _testInstance.NeedsToBeResolved.Should().BeTrue();
        }

        [Fact]
        public void Should_Set_Value_With_TrySet_If_JToken_Value_Can_Be_Interpreted()
        {
            _testInstance.NeedsToBeResolved = false;
            _testInstance.TrySet(JToken.Parse(@"{ ""values"": [ ""-3"", ""3"" ] }"));

            _testInstance.Selection.TryGetLeft(out var left).Should().BeTrue();
            left.Should().Contain(new[] { -3, 3 });

            _testInstance.NeedsToBeResolved.Should().BeTrue();
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
            Action func = () => _testInstance.TrySet(JToken.Parse(@"{ ""values"": [ ""-3a"", ""3a"" ] }"));

            func.Should().Throw<FormatException>();

            _testInstance.Selection.TryGetLeft(out var left).Should().BeTrue();
            left.Should().BeEmpty();

            _testInstance.NeedsToBeResolved.Should().BeFalse();
        }

        [Fact]
        public void Should_Not_Set_Value_With_TrySet_If_JToken_Has_Invalid_Content()
        {
            _testInstance.NeedsToBeResolved = false;
            _testInstance.TrySet(JToken.Parse(@"{ ""value"": [ -4, 4 ] }"));

            _testInstance.Selection.TryGetLeft(out var left).Should().BeTrue();
            left.Should().BeEmpty();

            _testInstance.NeedsToBeResolved.Should().BeFalse();
        }

        [Fact]
        public async Task Should_Return_Null_For_SetAvailableEntities_If_Queryable_Is_Empty()
        {
            _testInstance.NeedsToBeResolved = false;
            await _testInstance.SetEntities(Option.Some(new GenericSource[0].AsQueryable()), Option.None<IQueryable<GenericSource>>());

            _testInstance.Groups.Should().BeEmpty();
        }

        [Fact]
        public async Task Should_Return_Null_For_SetSelectableEntities_If_Queryable_Is_Empty()
        {
            _testInstance.NeedsToBeResolved = false;
            await _testInstance.SetEntities(Option.None<IQueryable<GenericSource>>(), Option.Some(new GenericSource[0].AsQueryable()));

            _testInstance.Groups.Should().BeEmpty();
        }

        [Fact]
        public void Should_Return_No_FilterExpression_If_There_Are_No_Values_As_Selection()
        {
            var items = new[]
            {
                new GenericSource { Int = -2, String = "Piza" },
                new GenericSource { Int = -1, String = "Chicken" },
                new GenericSource { Int = 0, String = "Chese" },
                new GenericSource { Int = 1, String = "Tuna" },
                new GenericSource { Int = 2, String = "Tun" }
            };

            var result = _testInstance.ExecuteFilter(items.AsQueryable());
            result.Should().HaveCount(5);
        }

        [Fact]
        public void Should_Return_No_FilterExpression_If_There_Are_No_Groups_As_Selection()
        {
            var items = new[]
            {
                new GenericSource { Int = -2, String = "Piza" },
                new GenericSource { Int = -1, String = "Chicken" },
                new GenericSource { Int = 0, String = "Chese" },
                new GenericSource { Int = 1, String = "Tuna" },
                new GenericSource { Int = 2, String = "Tun" }
            };

            _testInstance.SetGroups();
            var result = _testInstance.ExecuteFilter(items.AsQueryable());
            result.Should().HaveCount(5);
        }

        [Fact]
        public async Task Should_Support_Async_Queryables()
        {
            var items = new[]
            {
                new GenericSource { Int = -2, String = "Category1" },
                new GenericSource { Int = -1, String = "Category1" },
                new GenericSource { Int = 2, String = "Category1" },
                new GenericSource { Int = 0, String = "Category2" },
                new GenericSource { Int = 1, String = "Category2" },
                new GenericSource { Int = 2, String = "Category2" },
                new GenericSource { Int = 2, String = "Category2" },
                new GenericSource { Int = 3 }
            };

            await _testInstance.SetEntities(Option.Some(new List<GenericSource>(items).AsQueryable()), Option.Some(items.AsQueryable().Skip(1).Take(4)));

            var expected = new[]
            {
                new Group<string, int>
                {
                    Identifier = "Category1",
                    Values = new []
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
                            Value = 2
                        }
                    }
                },
                new Group<string, int>
                {
                    Identifier = "Category2",
                    Values = new []
                    {
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
                            CanBeSelected = true,
                            IsSelected = false,
                            Value = 2
                        }
                    }
                }
            };

            var expectedJson = JsonConvert.SerializeObject(expected);
            JsonConvert.SerializeObject(_testInstance.Groups).Should().Be(expectedJson);

            await _testInstance.SetEntities(Option.Some(new List<GenericSource>(new List<GenericSource>()).AsQueryable()), Option.None<IQueryable<GenericSource>>());

            _testInstance.Groups.Should().BeEmpty();
        }

        [Fact]
        public async Task Should_Get_Groups_If_No_Default_Group_Identifier_Is_Provided()
        {
            _testInstance.NeedsToBeResolved = false;
            var items = new[]
            {
                new GenericSource { Int = -2, String = "Category1" },
                new GenericSource { Int = -1, String = "Category1" },
                new GenericSource { Int = 2, String = "Category1" },
                new GenericSource { Int = 0, String = "Category2" },
                new GenericSource { Int = 1, String = "Category2" },
                new GenericSource { Int = 2, String = "Category2" },
                new GenericSource { Int = 2, String = "Category2" },
                new GenericSource { Int = 3 }
            };

            await _testInstance.SetEntities(Option.Some(items.AsQueryable()), Option.Some(items.AsQueryable().Skip(1).Take(4)));

            var expected = new[]
            {
                new Group<string, int>
                {
                    Identifier = "Category1",
                    Values = new []
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
                            Value = 2
                        }
                    }
                },
                new Group<string, int>
                {
                    Identifier = "Category2",
                    Values = new []
                    {
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
                            CanBeSelected = true,
                            IsSelected = false,
                            Value = 2
                        }
                    }
                }
            };

            var expectedJson = JsonConvert.SerializeObject(expected);
            JsonConvert.SerializeObject(_testInstance.Groups).Should().Be(expectedJson);
            _testInstance.NeedsToBeResolved.Should().BeFalse();
        }

        [Fact]
        public async Task Should_Get_Groups_With_Values_As_Selection_Input()
        {
            _testInstance.NeedsToBeResolved = false;
            _testInstance.UseDefaultGroup("Unknown");

            var items = new[]
            {
                new GenericSource { Int = -2, String = "Category1" },
                new GenericSource { Int = -1, String = "Category1" },
                new GenericSource { Int = 2, String = "Category1" },
                new GenericSource { Int = 0, String = "Category2" },
                new GenericSource { Int = 1, String = "Category2" },
                new GenericSource { Int = 2, String = "Category2" },
                new GenericSource { Int = 2, String = "Category2" },
                new GenericSource { Int = 3 }
            };

            await _testInstance.SetEntities(Option.Some(items.AsQueryable()), Option.None<IQueryable<GenericSource>>());
            _testInstance.Set(-1, 2);

            var expected = new[]
            {
                new Group<string, int>
                {
                    Identifier = "Category1",
                    Values = new []
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
                            IsSelected = true,
                            Value = 2
                        }
                    }
                },
                new Group<string, int>
                {
                    Identifier = "Category2",
                    Values = new []
                    {
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
                    }
                },
                new Group<string, int>
                {
                    Identifier = "Unknown",
                    Values = new []
                    {
                        new Item<int>
                        {
                            CanBeSelected = false,
                            IsSelected = false,
                            Value = 3
                        }
                    }
                }
            };

            var expectedJson = JsonConvert.SerializeObject(expected);
            JsonConvert.SerializeObject(_testInstance.Groups).Should().Be(expectedJson);
            _testInstance.NeedsToBeResolved.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Get_Groups_With_Groups_As_Selection_Input()
        {
            _testInstance.NeedsToBeResolved = false;
            _testInstance.UseDefaultGroup("Unknown");

            var items = new[]
            {
                new GenericSource { Int = -2, String = "Category1" },
                new GenericSource { Int = -1, String = "Category1" },
                new GenericSource { Int = 2, String = "Category1" },
                new GenericSource { Int = 0, String = "Category2" },
                new GenericSource { Int = 1, String = "Category2" },
                new GenericSource { Int = 2, String = "Category3" },
                new GenericSource { Int = 2, String = "Category4" },
                new GenericSource { Int = 3 }
            };

            await _testInstance.SetEntities(Option.Some(items.AsQueryable()), Option.None<IQueryable<GenericSource>>());
            _testInstance.SetGroups("Category2", "Category3", "InvalidCategory");

            var expected = new[]
            {
                new Group<string, int>
                {
                    Identifier = "Category1",
                    Values = new []
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
                            IsSelected = false,
                            Value = -1
                        },
                        new Item<int>
                        {
                            CanBeSelected = false,
                            IsSelected = false,
                            Value = 2
                        }
                    }
                },
                new Group<string, int>
                {
                    Identifier = "Category2",
                    Values = new []
                    {
                        new Item<int>
                        {
                            CanBeSelected = false,
                            IsSelected = true,
                            Value = 0
                        },
                        new Item<int>
                        {
                            CanBeSelected = false,
                            IsSelected = true,
                            Value = 1
                        }
                    }
                },
                new Group<string, int>
                {
                    Identifier = "Category3",
                    Values = new []
                    {
                        new Item<int>
                        {
                            CanBeSelected = false,
                            IsSelected = true,
                            Value = 2
                        }
                    }
                },
                new Group<string, int>
                {
                    Identifier = "Category4",
                    Values = new []
                    {
                        new Item<int>
                        {
                            CanBeSelected = false,
                            IsSelected = false,
                            Value = 2
                        }
                    }
                },
                new Group<string, int>
                {
                    Identifier = "Unknown",
                    Values = new []
                    {
                        new Item<int>
                        {
                            CanBeSelected = false,
                            IsSelected = false,
                            Value = 3
                        }
                    }
                }
            };

            var expectedJson = JsonConvert.SerializeObject(expected);
            JsonConvert.SerializeObject(_testInstance.Groups).Should().Be(expectedJson);
            _testInstance.NeedsToBeResolved.Should().BeTrue();
        }

        [Fact]
        public void Should_Get_Groups_If_There_Are_Only_Values_As_Selection_And_Default_Group_Is_Defined()
        {
            _testInstance.NeedsToBeResolved = false;
            _testInstance.UseDefaultGroup("Unknown");
            _testInstance.Set(-1, 2);

            var expected = new[]
            {
                new Group<string, int>
                {
                    Identifier = "Unknown",
                    Values = new []
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
                    }
                }
            };

            var expectedJson = JsonConvert.SerializeObject(expected);
            JsonConvert.SerializeObject(_testInstance.Groups).Should().Be(expectedJson);
            _testInstance.NeedsToBeResolved.Should().BeTrue();
        }

        [Fact]
        public void Should_Get_Groups_If_There_Are_Groups_As_Selection()
        {
            _testInstance.NeedsToBeResolved = false;
            _testInstance.SetGroups("Group1", "Group2");

            var expected = new[]
            {
                new Group<string, int>
                {
                    Identifier = "Group1",
                    Values = null
                },
                new Group<string, int>
                {
                    Identifier = "Group2",
                    Values = null
                }
            };

            var expectedJson = JsonConvert.SerializeObject(expected);
            JsonConvert.SerializeObject(_testInstance.Groups).Should().Be(expectedJson);
            _testInstance.NeedsToBeResolved.Should().BeTrue();
        }

        [Fact]
        public void Should_Not_Get_Values_If_There_Are_Only_Values_As_Selection_And_Default_Group_Is_Undefined()
        {
            _testInstance.NeedsToBeResolved = false;
            _testInstance.Set(-1, 2);

            var expected = new Group<string, int>[] {};

            var expectedJson = JsonConvert.SerializeObject(expected);
            JsonConvert.SerializeObject(_testInstance.Groups).Should().Be(expectedJson);
            _testInstance.NeedsToBeResolved.Should().BeTrue();
        }
    }
}
