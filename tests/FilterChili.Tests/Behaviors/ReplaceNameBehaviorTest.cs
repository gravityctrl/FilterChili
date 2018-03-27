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
using GravityCTRL.FilterChili.Behaviors;
using GravityCTRL.FilterChili.Resolvers.Range;
using GravityCTRL.FilterChili.Tests.TestSupport.Models;
using Xunit;

namespace GravityCTRL.FilterChili.Tests.Behaviors
{
    public class ReplaceNameBehaviorTest
    {
        [Fact]
        public void Should_Update_Name_Of_Resolver_When_Calling_Use_Name_And_Return_Same_Instance()
        {
            RangeResolver<GenericSource, int> testResolver = new IntRangeResolver<GenericSource>(source => source.Int);
            var testInstance = new ReplaceNameBehavior<GenericSource, int>("SomeName");
            testResolver.Name.Should().Be("Int");

            testInstance.Apply(testResolver);
            testResolver.Name.Should().Be("SomeName");
        }
    }
}
