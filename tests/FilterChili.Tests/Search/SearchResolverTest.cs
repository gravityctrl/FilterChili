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
using GravityCTRL.FilterChili.Search;
using Xunit;

namespace GravityCTRL.FilterChili.Tests.Search
{
    public sealed class SearchResolverTest
    {
        private readonly SearchResolver<TestSource> _testInstance;

        public SearchResolverTest()
        {
            _testInstance = new SearchResolver<TestSource>();
        }

        [Theory]
        [InlineData("abc", new[] { "abc" })]
        [InlineData("abc,def", new[] { "abc", "def" })]
        [InlineData("vwx", new[] { "def", "jkl", "pqr" })]
        [InlineData("vwx,stu", new[] { "abc", "def", "ghi", "jkl", "mno", "pqr" })]
        [InlineData("-abc", new[] { "def", "ghi", "jkl", "mno", "pqr" })]
        [InlineData("-abc,def", new[] { "ghi", "jkl", "mno", "pqr" })]
        [InlineData("-vwx", new[] { "abc", "ghi", "mno" })]
        [InlineData("-vwx,stu", new string[] {})]
        [InlineData("name:abc", new[] { "abc" })]
        [InlineData("name:abc,def", new[] { "abc", "def" })]
        [InlineData("category:vwx", new[] { "def", "jkl", "pqr" })]
        [InlineData("category:vwx,stu", new[] { "abc", "def", "ghi", "jkl", "mno", "pqr" })]
        [InlineData("-name:abc", new[] { "def", "ghi", "jkl", "mno", "pqr" })]
        [InlineData("-name:abc,def", new[] { "ghi", "jkl", "mno", "pqr" })]
        [InlineData("-category:vwx", new[] { "abc", "ghi", "mno" })]
        [InlineData("-category:vwx,stu", new string[] {})]
        [InlineData("notthere:abc", new[] { "abc", "def", "ghi", "jkl", "mno", "pqr" })]
        [InlineData("notthere:abc,def", new[] { "abc", "def", "ghi", "jkl", "mno", "pqr" })]
        [InlineData("-notthere:abc", new[] { "abc", "def", "ghi", "jkl", "mno", "pqr" })]
        [InlineData("-notthere:abc,def", new[] { "abc", "def", "ghi", "jkl", "mno", "pqr" })]
        public void Should_Resolve_Search_Correctly(string searchString, string[] expectedResults)
        {
            var specification1 = new SearchSpecification<TestSource>(source => source.Name);
            var specification2 = new SearchSpecification<TestSource>(source => source.Category).UseEquals();

            _testInstance.AddSearcher(specification1);
            _testInstance.AddSearcher(specification2);

            _testInstance.SetSearchString(searchString);

            var items = new[]
            {
                new TestSource { Name = "abc", Category = "stu" },
                new TestSource { Name = "def", Category = "vwx" },
                new TestSource { Name = "ghi", Category = "stu" },
                new TestSource { Name = "jkl", Category = "vwx" },
                new TestSource { Name = "mno", Category = "stu" },
                new TestSource { Name = "pqr", Category = "vwx" }
            };

            var result1 = _testInstance.ApplySearch(items.AsQueryable());
            result1.Select(element => element.Name).Should().BeEquivalentTo(expectedResults);

            _testInstance.SetSearchString(searchString);
            var result2 = _testInstance.ApplySearch(items.AsQueryable());
            result2.Select(element => element.Name).Should().BeEquivalentTo(expectedResults);
        }

        [Theory]
        [InlineData("abc", new[] { "abc", "def", "ghi", "jkl", "mno", "pqr" })]
        [InlineData("abc,def", new[] { "abc", "def", "ghi", "jkl", "mno", "pqr" })]
        [InlineData("vwx", new[] { "abc", "def", "ghi", "jkl", "mno", "pqr" })]
        [InlineData("vwx,stu", new[] { "abc", "def", "ghi", "jkl", "mno", "pqr" })]
        [InlineData("-abc", new[] { "abc", "def", "ghi", "jkl", "mno", "pqr" })]
        [InlineData("-abc,def", new[] { "abc", "def", "ghi", "jkl", "mno", "pqr" })]
        [InlineData("-vwx", new[] { "abc", "def", "ghi", "jkl", "mno", "pqr" })]
        [InlineData("-vwx,stu", new[] { "abc", "def", "ghi", "jkl", "mno", "pqr" })]
        [InlineData("name:abc", new[] { "abc", "def", "ghi", "jkl", "mno", "pqr" })]
        [InlineData("name:abc,def", new[] { "abc", "def", "ghi", "jkl", "mno", "pqr" })]
        [InlineData("category:vwx", new[] { "abc", "def", "ghi", "jkl", "mno", "pqr" })]
        [InlineData("category:vwx,stu", new[] { "abc", "def", "ghi", "jkl", "mno", "pqr" })]
        [InlineData("-name:abc", new[] { "abc", "def", "ghi", "jkl", "mno", "pqr" })]
        [InlineData("-name:abc,def", new[] { "abc", "def", "ghi", "jkl", "mno", "pqr" })]
        [InlineData("-category:vwx", new[] { "abc", "def", "ghi", "jkl", "mno", "pqr" })]
        [InlineData("-category:vwx,stu", new[] { "abc", "def", "ghi", "jkl", "mno", "pqr" })]
        [InlineData("notthere:abc", new[] { "abc", "def", "ghi", "jkl", "mno", "pqr" })]
        [InlineData("notthere:abc,def", new[] { "abc", "def", "ghi", "jkl", "mno", "pqr" })]
        [InlineData("-notthere:abc", new[] { "abc", "def", "ghi", "jkl", "mno", "pqr" })]
        [InlineData("-notthere:abc,def", new[] { "abc", "def", "ghi", "jkl", "mno", "pqr" })]
        public void Should_Resolve_Search_Correctly_If_No_Searchers_Are_Defined(string searchString, string[] expectedResults)
        {
            _testInstance.SetSearchString(searchString);

            var items = new[]
            {
                new TestSource { Name = "abc", Category = "stu" },
                new TestSource { Name = "def", Category = "vwx" },
                new TestSource { Name = "ghi", Category = "stu" },
                new TestSource { Name = "jkl", Category = "vwx" },
                new TestSource { Name = "mno", Category = "stu" },
                new TestSource { Name = "pqr", Category = "vwx" }
            };

            var result = _testInstance.ApplySearch(items.AsQueryable());
            result.Select(element => element.Name).Should().BeEquivalentTo(expectedResults);
        }

        private sealed class TestSource
        {
            public string Name { get; set; }
            public string Category { get; set; }
        }
    }
}
