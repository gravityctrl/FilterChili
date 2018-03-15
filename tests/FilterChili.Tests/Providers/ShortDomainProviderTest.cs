﻿// This file is part of FilterChili.
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
using GravityCTRL.FilterChili.Resolvers.Comparison;
using GravityCTRL.FilterChili.Resolvers.Range;
using GravityCTRL.FilterChili.Selectors;
using GravityCTRL.FilterChili.Tests.TestSupport.Models;
using Xunit;

namespace GravityCTRL.FilterChili.Tests.Providers
{
    public class ShortDomainProviderTest
    {
        private const string TEST_NAME = "TestName";

        private readonly ShortFilterSelector<GenericSource> _testInstance;

        public ShortDomainProviderTest()
        {
            _testInstance = new ShortFilterSelector<GenericSource>(p => p.Short);
        }

        [Fact]
        public void Should_Return_Range_Resolver()
        {
            var result = _testInstance.Range(TEST_NAME);
            result.Should().BeOfType<ShortRangeResolver<GenericSource>>();
            result.Name.Should().Be(TEST_NAME);
            result.FilterType.Should().Be("Range");
        }

        [Fact]
        public void Should_Return_GreaterThan_Resolver()
        {
            var result = _testInstance.GreaterThan(TEST_NAME);
            result.Should().BeOfType<ShortComparisonResolver<GenericSource>>();
            result.Name.Should().Be(TEST_NAME);
            result.FilterType.Should().Be("GreaterThan");
        }

        [Fact]
        public void Should_Return_LessThan_Resolver()
        {
            var result = _testInstance.LessThan(TEST_NAME);
            result.Should().BeOfType<ShortComparisonResolver<GenericSource>>();
            result.Name.Should().Be(TEST_NAME);
            result.FilterType.Should().Be("LessThan");
        }

        [Fact]
        public void Should_Return_GreaterThanOrEqual_Resolver()
        {
            var result = _testInstance.GreaterThanOrEqual(TEST_NAME);
            result.Should().BeOfType<ShortComparisonResolver<GenericSource>>();
            result.Name.Should().Be(TEST_NAME);
            result.FilterType.Should().Be("GreaterThanOrEqual");
        }

        [Fact]
        public void Should_Return_LessThanOrEqual_Resolver()
        {
            var result = _testInstance.LessThanOrEqual(TEST_NAME);
            result.Should().BeOfType<ShortComparisonResolver<GenericSource>>();
            result.Name.Should().Be(TEST_NAME);
            result.FilterType.Should().Be("LessThanOrEqual");
        }
    }
}
