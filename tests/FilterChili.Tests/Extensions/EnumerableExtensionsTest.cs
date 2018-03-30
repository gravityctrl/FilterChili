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

using System.Linq.Expressions;
using FluentAssertions;
using GravityCTRL.FilterChili.Extensions;
using Xunit;

namespace GravityCTRL.FilterChili.Tests.Extensions
{
    public sealed class EnumerableExtensionsTest
    {
        [Fact]
        public void Should_Return_Null_If_Expressions_Are_Empty()
        {
            new Expression[0].Or().Should().BeNull();
            new Expression[0].And().Should().BeNull();
        }

        [Fact]
        public void Should_Return_Same_Expression_If_Expressions_Only_Contain_The_One()
        {
            var expression = Expression.Constant(0);
            new Expression[] { expression }.Or().Should().Be(expression);
            new Expression[] { expression }.And().Should().Be(expression);
        }

        [Fact]
        public void Should_Combine_Expressions_Via_Or_If_Expressions_Have_More_Than_One_Items()
        {
            var expected = Expression.Or(
                Expression.Or(
                    Expression.Constant(0), 
                    Expression.Constant(1)
                ), 
                Expression.Constant(2)
            ).ToString();

            var expressions = new Expression[] { Expression.Constant(0), Expression.Constant(1), Expression.Constant(2) };

            // ReSharper disable once PossibleNullReferenceException
            expressions.Or().ToString().Should().Be(expected);
        }

        [Fact]
        public void Should_Combine_Expressions_Via_And_If_Expressions_Have_More_Than_One_Items()
        {
            var expected = Expression.And(
                Expression.And(
                    Expression.Constant(0),
                    Expression.Constant(1)
                ),
                Expression.Constant(2)
            ).ToString();

            var expressions = new Expression[] { Expression.Constant(0), Expression.Constant(1), Expression.Constant(2) };

            // ReSharper disable once PossibleNullReferenceException
            expressions.And().ToString().Should().Be(expected);
        }
    }
}
