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

using System.Linq;
using FluentAssertions;
using GravityCTRL.FilterChili.Enums;
using GravityCTRL.FilterChili.Resolvers.List;
using GravityCTRL.FilterChili.Tests.TestSupport.Models;
using Xunit;

namespace GravityCTRL.FilterChili.Tests.Resolvers.List
{
    public class StringListResolverTest
    {
        private readonly StringListResolver<GenericSource> _testInstance;

        public StringListResolverTest()
        {
            _testInstance = new StringListResolver<GenericSource>(source => source.String, StringComparisonStrategy.Equals);
        }

        [Fact]
        public void Should_Return_Null_If_There_Are_No_Items()
        {
            _testInstance.ComparisonStrategy = StringComparisonStrategy.Equals;

            var items = new[]
            {
                new GenericSource { String = "Piza" },
                new GenericSource { String = "Chicken" },
                new GenericSource { String = "Chese" },
                new GenericSource { String = "Tuna" },
                new GenericSource { String = "Tun" }
            };

            var result = _testInstance.ExecuteFilter(items.AsQueryable());
            result.Should().HaveCount(5);
        }

        [Fact]
        public void Should_Use_ComparisonStrategy_Equals_Correctly()
        {
            _testInstance.ComparisonStrategy = StringComparisonStrategy.Equals;

            var items = new[]
            {
                new GenericSource { String = "Piza" },
                new GenericSource { String = "Chicken" },
                new GenericSource { String = "Chese" },
                new GenericSource { String = "Tuna" },
                new GenericSource { String = "Tun" }
            };

            _testInstance.Set("Pizza", "Chicken", "Cheese", "Tun");
            var result = _testInstance.ExecuteFilter(items.AsQueryable());
            result.Should().HaveCount(2);
        }

        [Fact]
        public void Should_Use_ComparisonStrategy_Contains_Correctly()
        {
            _testInstance.ComparisonStrategy = StringComparisonStrategy.Contains;

            var items = new[]
            {
                new GenericSource { String = "Piza" },
                new GenericSource { String = "Chicken" },
                new GenericSource { String = "Chese" },
                new GenericSource { String = "Tuna" },
                new GenericSource { String = "Tun" }
            };

            _testInstance.Set("Pizza", "Chicken", "Cheese", "Tun");
            var result = _testInstance.ExecuteFilter(items.AsQueryable());
            result.Should().HaveCount(3);
        }

        [Fact]
        public void Should_Use_ComparisonStrategy_GermanSoundex_Correctly()
        {
            _testInstance.ComparisonStrategy = StringComparisonStrategy.GermanSoundex;

            var items = new[]
            {
                new GenericSource { String = "Piza" },
                new GenericSource { String = "Chicken" },
                new GenericSource { String = "Chese" },
                new GenericSource { String = "Tuna" },
                new GenericSource { String = "Tun" }
            };

            _testInstance.Set("Bizza", "Chicken", "Cheese", "Dun");
            var result = _testInstance.ExecuteFilter(items.AsQueryable());
            result.Should().HaveCount(5);
        }

        [Fact]
        public void Should_Use_ComparisonStrategy_Soundex_Correctly()
        {
            _testInstance.ComparisonStrategy = StringComparisonStrategy.Soundex;

            var items = new[]
            {
                new GenericSource { String = "Piza" },
                new GenericSource { String = "Chicken" },
                new GenericSource { String = "Chese" },
                new GenericSource { String = "Tuna" },
                new GenericSource { String = "Tun" }
            };

            _testInstance.Set("Bizza", "Chicken", "Cheese", "Dun");
            var result = _testInstance.ExecuteFilter(items.AsQueryable());
            result.Should().HaveCount(2);
        }
    }
}
