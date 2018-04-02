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
using FluentAssertions;
using GravityCTRL.FilterChili.Models;
using GravityCTRL.FilterChili.Tests.TestSupport.Models;
using Xunit;

namespace GravityCTRL.FilterChili.Tests.Models
{
    public sealed class OptionTest
    {
        [Fact]
        public void Should_Create_None_Option()
        {
            var testInstance = Option.None<GenericSource>();
            
            testInstance.Should().BeOfType<None<GenericSource>>();
            testInstance.TryGetValue(out var value).Should().BeFalse();
            value.Should().Be(null);
        }

        [Fact]
        public void Should_Create_Some_Option()
        {
            var input = new GenericSource();
            var testInstance = Option.Some(input);

            testInstance.Should().BeOfType<Some<GenericSource>>();
            testInstance.TryGetValue(out var value).Should().BeTrue();
            value.Should().Be(input);
        }

        [Fact]
        public void Should_Throw_ArgumentNullException_When_Trying_To_Create_Some_With_Null()
        {
            Action action = () => Option.Some((GenericSource)null);
            action.Should()
                .Throw<ArgumentNullException>()
                .WithMessage($"Value cannot be null.{Environment.NewLine}Parameter name: value");
        }

        [Fact]
        public void Should_Create_Some_Option_If_Input_Not_Null()
        {
            var input = new GenericSource();
            var testInstance = Option.Maybe(input);

            testInstance.Should().BeOfType<Some<GenericSource>>();
            testInstance.TryGetValue(out var value).Should().BeTrue();
            value.Should().Be(input);
        }

        [Fact]
        public void Should_Create_None_Option_If_Input_Null()
        {
            var testInstance = Option.Maybe((GenericSource) null);

            testInstance.Should().BeOfType<None<GenericSource>>();
            testInstance.TryGetValue(out var value).Should().BeFalse();
            value.Should().Be(null);
        }

        [Fact]
        public void Should_Create_Some_Option_Implicitly()
        {
            var input = new GenericSource();
            Option<GenericSource> testInstance = input;

            testInstance.Should().BeOfType<Some<GenericSource>>();
            testInstance.TryGetValue(out var value).Should().BeTrue();
            value.Should().Be(input);
        }

        [Fact]
        public void Should_Create_None_Option_Implicitly()
        {
            Option<GenericSource> testInstance = (GenericSource) null;

            testInstance.Should().BeOfType<None<GenericSource>>();
            testInstance.TryGetValue(out var value).Should().BeFalse();
            value.Should().Be(null);
        }
    }
}
