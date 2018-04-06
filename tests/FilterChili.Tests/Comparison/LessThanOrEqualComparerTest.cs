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
using GravityCTRL.FilterChili.Comparison;
using GravityCTRL.FilterChili.Models;
using GravityCTRL.FilterChili.Tests.Shared.Models;
using Xunit;

namespace GravityCTRL.FilterChili.Tests.Comparison
{
    public sealed class LessThanOrEqualComparerTest
    {
        private readonly LessThanOrEqualComparer<Product, int> _testInstance;

        public LessThanOrEqualComparerTest()
        {
            _testInstance = new LessThanOrEqualComparer<Product, int>(0);
        }

        [Fact]
        public void Should_Have_Correct_FilterType()
        {
            _testInstance.FilterType.Should().Be("LessThanOrEqual");
        }

        [Fact]
        public void Should_Return_Instance_If_SelectedValue_Is_Greater_Than_Min()
        {
            var expression = _testInstance.FilterExpression(p => p.Id, -1);
            expression.TryGetValue(out _).Should().BeTrue();
        }

        [Fact]
        public void Should_Return_Null_If_SelectedValue_Is_Equal_To_Min()
        {
            var expression = _testInstance.FilterExpression(p => p.Id, 0);
            expression.TryGetValue(out _).Should().BeFalse();
        }

        [Fact]
        public void Should_Return_Null_If_SelectedValue_Is_Less_Than_Min()
        {
            var expression = _testInstance.FilterExpression(p => p.Id, 1);
            expression.TryGetValue(out _).Should().BeFalse();
        }
    }
}
