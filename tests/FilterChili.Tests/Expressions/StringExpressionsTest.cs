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

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GravityCTRL.FilterChili.Expressions;
using GravityCTRL.FilterChili.Tests.TestSupport.Models;
using Xunit;

namespace GravityCTRL.FilterChili.Tests.Expressions
{
    public sealed class StringExpressionsTest
    {
        [Fact]
        public void Should_Use_ComparisonStrategy_Equals_Correctly()
        {
            var values = new List<string> { "Pizza", "Chicken", "Cheese", "Tun" };
            var expression = StringExpressions.CreateFilterExpression<GenericSource>(values, StringComparisonStrategy.Equals, source => source.String);

            var items = new[]
            {
                new GenericSource { String = "Piza" },
                new GenericSource { String = "Chicken" },
                new GenericSource { String = "Chese" },
                new GenericSource { String = "Tuna" },
                new GenericSource { String = "Tun" }
            };

            // ReSharper disable once PossibleNullReferenceException
            var result = items.Where(expression.Compile());
            result.Should().HaveCount(2);
        }

        [Fact]
        public void Should_Use_ComparisonStrategy_Contains_Correctly()
        {
            var values = new List<string> { "Pizza", "Chicken", "Cheese", "Tun" };
            var expression = StringExpressions.CreateFilterExpression<GenericSource>(values, StringComparisonStrategy.Contains, source => source.String);

            var items = new[]
            {
                new GenericSource { String = "Piza" },
                new GenericSource { String = "Chicken" },
                new GenericSource { String = "Chese" },
                new GenericSource { String = "Tuna" },
                new GenericSource { String = "Tun" }
            };

            // ReSharper disable once PossibleNullReferenceException
            var result = items.Where(expression.Compile());
            result.Should().HaveCount(3);
        }

        [Fact]
        public void Should_Use_ComparisonStrategy_GermanSoundex_Correctly()
        {
            var values = new List<string> { "Bizza", "Chicken", "Cheese", "Dun" };
            var expression = StringExpressions.CreateFilterExpression<GenericSource>(values, StringComparisonStrategy.GermanSoundex, source => source.String);

            var items = new[]
            {
                new GenericSource { String = "Piza" },
                new GenericSource { String = "Chicken" },
                new GenericSource { String = "Chese" },
                new GenericSource { String = "Tuna" },
                new GenericSource { String = "Tun" }
            };

            // ReSharper disable once PossibleNullReferenceException
            var result = items.Where(expression.Compile());
            result.Should().HaveCount(5);
        }

        [Fact]
        public void Should_Use_ComparisonStrategy_Soundex_Correctly()
        {
            var values = new List<string> { "Bizza", "Chicken", "Cheese", "Dun" };
            var expression = StringExpressions.CreateFilterExpression<GenericSource>(values, StringComparisonStrategy.Soundex, source => source.String);

            var items = new[]
            {
                new GenericSource { String = "Piza" },
                new GenericSource { String = "Chicken" },
                new GenericSource { String = "Chese" },
                new GenericSource { String = "Tuna" },
                new GenericSource { String = "Tun" }
            };

            // ReSharper disable once PossibleNullReferenceException
            var result = items.Where(expression.Compile());
            result.Should().HaveCount(2);
        }
    }
}
