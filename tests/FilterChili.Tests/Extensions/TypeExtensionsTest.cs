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
using GravityCTRL.FilterChili.Extensions;
using GravityCTRL.FilterChili.Tests.Shared.Models;
using JetBrains.Annotations;
using Xunit;

namespace GravityCTRL.FilterChili.Tests.Extensions
{
    public class TypeExtensionsTest
    {
        [Fact]
        public void Should_Create_Correct_Formatted_Name_For_Type()
        {
            var type = typeof(TestClass1<TestClass2<float, TestClass3<int>>, TestClass2<double, bool>, string>);
            type.FormattedName().Should().Be("TestClass1<TestClass2<Single,TestClass3<Int32>>,TestClass2<Double,Boolean>,String>");
        }

        [Fact]
        public void Should_Resolve_Expression_Target_Names()
        {
            Expression<Func<Product, int>> expression = product => product.Sold;
            expression.Name().Should().Be("Sold");
        }

        // ReSharper disable UnusedTypeParameter
        [UsedImplicitly] private class TestClass1<T1, T2, T3> {}
        [UsedImplicitly] private class TestClass2<T1, T2> {}
        [UsedImplicitly] private class TestClass3<T1> {}
        // ReSharper restore UnusedTypeParameter
    }
}
