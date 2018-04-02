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
using GravityCTRL.FilterChili.Tests.TestSupport.Models;
using Xunit;

namespace GravityCTRL.FilterChili.Tests.Resolvers
{
    public sealed class FilterResolverTest
    {
        [Fact]
        public void Should_Update_Name_Of_Resolver_When_Calling_UseName_And_Return_Same_Instance()
        {
            var testInstance = new RangeResolver<GenericSource, int>(_ => 1, int.MinValue, int.MaxValue);
            testInstance.Name.Should().Be(null);

            testInstance.UseName("SomeName").Should().Be(testInstance);
            testInstance.Name.Should().Be("SomeName");
        }

        [Fact]
        public void Should_Replace_Existing_Behavior_If_Behavior_Class_Is_Same_Type()
        {
            var testInstance = new RangeResolver<GenericSource, int>(_ => 1, int.MinValue, int.MaxValue);
            testInstance.Name.Should().Be(null);

            testInstance.UseName("SomeName").Should().Be(testInstance);
            testInstance.Name.Should().Be("SomeName");

            testInstance.UseName("SomeName2").Should().Be(testInstance);
            testInstance.Name.Should().Be("SomeName2");
        }

        [Fact]
        public void Should_Update_Calculation_Strategy_When_Calling_UseStrategy_And_Return_Same_Instance()
        {
            var testInstance = new RangeResolver<GenericSource, int>(_ => 1, int.MinValue, int.MaxValue);
            testInstance.Name.Should().Be(null);

            testInstance.UseStrategy(CalculationStrategy.AvailableValues).Should().Be(testInstance);
            testInstance.CalculationStrategy.Should().Be(CalculationStrategy.AvailableValues);

            testInstance.UseStrategy(CalculationStrategy.SelectableValues).Should().Be(testInstance);
            testInstance.CalculationStrategy.Should().Be(CalculationStrategy.SelectableValues);
        }
    }
}
