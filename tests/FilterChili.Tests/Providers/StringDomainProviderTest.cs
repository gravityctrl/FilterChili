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
using GravityCTRL.FilterChili.Enums;
using GravityCTRL.FilterChili.Providers;
using GravityCTRL.FilterChili.Resolvers.List;
using GravityCTRL.FilterChili.Tests.Models;
using Xunit;

namespace GravityCTRL.FilterChili.Tests.Providers
{
    public class StringDomainProviderTest
    {
        private const string TEST_NAME = "TestName";

        private readonly StringDomainProvider<GenericSource> _testInstance;

        public StringDomainProviderTest()
        {
            _testInstance = new StringDomainProvider<GenericSource>(p => p.String);
        }

        [Fact]
        public void Should_Return_List_Resolver()
        {
            var result = _testInstance.List(TEST_NAME);
            result.Should().BeOfType<StringListResolver<GenericSource>>();
            result.Name.Should().Be(TEST_NAME);
            result.FilterType.Should().Be("List");
            result.ComparisonStrategy.Should().Be(StringComparisonStrategy.Equals);
        }

        [Fact]
        public void Should_Accept_Other_Comparison_Strategy()
        {
            var result = _testInstance.List(TEST_NAME, StringComparisonStrategy.Contains);
            result.ComparisonStrategy.Should().Be(StringComparisonStrategy.Contains);
        }
    }
}
