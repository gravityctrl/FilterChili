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
using System.Linq.Expressions;
using FluentAssertions;
using GravityCTRL.FilterChili.Tests.TestSupport.Models;
using Xunit;

namespace GravityCTRL.FilterChili.Tests.Resolvers
{
    public class DomainResolverTest
    {
        [Fact]
        public void Should_Update_Name_Of_Resolver_When_Calling_Use_Name_And_Return_Same_Instance()
        {
            var testInstance = new TestResolver(_ => 1);
            testInstance.Name.Should().Be(null);

            testInstance.UseName("SomeName").Should().Be(testInstance);
            testInstance.Name.Should().Be(null);

            testInstance.ApplyBehaviors();
            testInstance.Name.Should().Be("SomeName");
        }

        [Fact]
        public void Should_Replace_Existing_Behavior_If_Behavior_Class_Is_Same_Type()
        {
            var testInstance = new TestResolver(_ => 1);
            testInstance.Name.Should().Be(null);

            testInstance.UseName("SomeName").Should().Be(testInstance);
            testInstance.ApplyBehaviors();
            testInstance.Name.Should().Be("SomeName");

            testInstance.UseName("SomeName2").Should().Be(testInstance);
            testInstance.ApplyBehaviors();
            testInstance.Name.Should().Be("SomeName2");
        }

        private class TestResolver : RangeResolver<GenericSource, int>
        {
            internal TestResolver(Expression<Func<GenericSource, int>> selector) : base(selector, 1, 2) { }
        }
    }
}
