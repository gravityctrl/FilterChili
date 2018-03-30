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

using FluentAssertions;
using GravityCTRL.FilterChili.Selectors;
using GravityCTRL.FilterChili.Tests.TestSupport.Models;
using Xunit;

namespace GravityCTRL.FilterChili.Tests.Selectors
{
    public sealed class StringFilterSelectorTest
    {
        private const string TEST_NAME = "String";

        private readonly StringFilterSelector<GenericSource> _testInstance;

        public StringFilterSelectorTest()
        {
            _testInstance = new StringFilterSelector<GenericSource>(p => p.String);
        }

        [Fact]
        public void Should_Return_List_Resolver()
        {
            var result = _testInstance.WithList();
            result.Should().BeOfType<ListResolver<GenericSource, string>>();
            result.Name.Should().Be(TEST_NAME);
            result.FilterType.Should().Be("List");
        }

        [Fact]
        public void Should_Return_Group_Resolver()
        {
            var result = _testInstance.WithGroup(source => source.Decimal);
            result.Should().BeOfType<GroupResolver<GenericSource, string, decimal>>();
            result.Name.Should().Be(TEST_NAME);
            result.FilterType.Should().Be("Group");
        }
    }
}
