using System.Linq;
using FluentAssertions;
using GravityCTRL.FilterChili.Enums;
using GravityCTRL.FilterChili.Resolvers.List;
using GravityCTRL.FilterChili.Tests.Models;
using Xunit;

namespace GravityCTRL.FilterChili.Tests.Resolvers.List
{
    public class StringListResolverTest
    {
        private const string TEST_NAME = "TestName";

        private readonly StringListResolver<GenericSource> _testInstance;

        public StringListResolverTest()
        {
            _testInstance = new StringListResolver<GenericSource>(TEST_NAME, source => source.String, StringComparisonStrategy.Equals);
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
