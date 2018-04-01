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
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using GravityCTRL.FilterChili.Exceptions;
using GravityCTRL.FilterChili.Models;
using GravityCTRL.FilterChili.Selectors;
using GravityCTRL.FilterChili.Tests.TestSupport.Models;
using Newtonsoft.Json;
using Xunit;

namespace GravityCTRL.FilterChili.Tests
{
    public sealed class ContextOptionsTest
    {
        private readonly ContextOptions<GenericSource> _testInstance;

        private bool _contextOptionsInitialized;

        public ContextOptionsTest()
        {
            Randomizer.Seed = new Random(0);

            var testGenericSources = new Faker<GenericSource>();
            testGenericSources.RuleFor(entity => entity.Int, faker => faker.Random.Int(0, 10));
            testGenericSources.RuleFor(entity => entity.Double, faker => faker.Random.Double(0, 20));
            testGenericSources.RuleFor(entity => entity.Float, faker => faker.Random.Float(0, 30));
            testGenericSources.RuleFor(entity => entity.String, faker => faker.Commerce.Product());
            var items = testGenericSources.GenerateLazy(20).ToList();

            var queryable = items.AsQueryable();
            _testInstance = new ContextOptions<GenericSource>(queryable, options =>
            {
                _contextOptionsInitialized = true;
            });
        }

        [Fact]
        public void Should_Initialize_Correctly()
        {
            _contextOptionsInitialized.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Be_Able_To_Use_FilterSelectors()
        {
            _testInstance.Filter(source => source.Byte).Should().BeOfType<ByteFilterSelector<GenericSource>>();
            _testInstance.Filter(source => source.Char).Should().BeOfType<CharFilterSelector<GenericSource>>();
            _testInstance.Filter(source => source.Decimal).Should().BeOfType<DecimalFilterSelector<GenericSource>>();
            _testInstance.Filter(source => source.Double).Should().BeOfType<DoubleFilterSelector<GenericSource>>();
            _testInstance.Filter(source => source.Float).Should().BeOfType<FloatFilterSelector<GenericSource>>();
            _testInstance.Filter(source => source.Int).Should().BeOfType<IntFilterSelector<GenericSource>>();
            _testInstance.Filter(source => source.Long).Should().BeOfType<LongFilterSelector<GenericSource>>();
            _testInstance.Filter(source => source.SByte).Should().BeOfType<SByteFilterSelector<GenericSource>>();
            _testInstance.Filter(source => source.Short).Should().BeOfType<ShortFilterSelector<GenericSource>>();
            _testInstance.Filter(source => source.String).Should().BeOfType<StringFilterSelector<GenericSource>>();
            _testInstance.Filter(source => source.UInt).Should().BeOfType<UIntFilterSelector<GenericSource>>();
            _testInstance.Filter(source => source.ULong).Should().BeOfType<ULongFilterSelector<GenericSource>>();
            _testInstance.Filter(source => source.UShort).Should().BeOfType<UShortFilterSelector<GenericSource>>();

            var domains = await _testInstance.Domains();

            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Action enumerateAction = () => domains.ToList();
            enumerateAction.Should().Throw<MissingResolverException>().Where(ex => ex.Message.EndsWith("FilterSelector<GenericSource>", StringComparison.Ordinal));
        }

        [Fact]
        public async Task Should_Resolve_Domains_Independent_From_Setting_Filters_Order()
        {
            var filter1 = _testInstance.Filter(source => source.Int).WithGreaterThan();
            var filter2 = _testInstance.Filter(source => source.Double).WithLessThanOrEqual();
            var filter3 = _testInstance.Filter(source => source.Float).WithRange();

            filter1.Set(1);
            await _testInstance.Domains();
            filter2.Set(19);
            await _testInstance.Domains();
            filter3.Set(3, 27);

            var testResult1 = JsonConvert.SerializeObject(await _testInstance.Domains());

            filter1.Set(1);
            filter2.Set(19);
            filter3.Set(3, 27);

            var testResult2 = JsonConvert.SerializeObject(await _testInstance.Domains());

            testResult1.Should().Be(testResult2);
        }

        [Fact]
        public void Should_Get_Filter_By_Name()
        {
            _testInstance.Filter(source => source.Int).WithGreaterThan();
            _testInstance.Filter(source => source.Double).WithLessThanOrEqual();
            _testInstance.Filter(source => source.Float).WithRange();

            _testInstance.GetFilter("Int").TryGetValue(out var intFilter).Should().BeTrue();
            intFilter.Domain().Name.Should().Be("Int");

            _testInstance.GetFilter("Double").TryGetValue(out var doubleFilter).Should().BeTrue();
            doubleFilter.Domain().Name.Should().Be("Double");

            _testInstance.GetFilter("Float").TryGetValue(out var floatFilter).Should().BeTrue();
            floatFilter.Domain().Name.Should().Be("Float");

            _testInstance.GetFilter("Byte").TryGetValue(out var _).Should().BeFalse();
        }

        [Fact]
        public void Should_Return_Expected_Entities_When_Applying_Filters()
        {
            var filter1 = _testInstance.Filter(source => source.Int).WithGreaterThan();
            var filter2 = _testInstance.Filter(source => source.Double).WithLessThanOrEqual();
            var filter3 = _testInstance.Filter(source => source.Float).WithRange();

            filter1.Set(3);
            filter2.Set(15);
            filter3.Set(5, 25);

            var results = _testInstance.ApplyFilters().ToList();
            results.Should().NotContain(entity => entity.Int <= 3);
            results.Should().NotContain(entity => entity.Double > 15);
            results.Should().NotContain(entity => entity.Float < 5 && entity.Float > 25);
        }

        [Fact]
        public void Should_Return_Expected_Entities_When_Applying_Search()
        {
            _testInstance.Search(source => source.String).UseEquals();
            _testInstance.Search(source => source.Int.ToString());

            _testInstance.SetSearch("cheese");

            var results = _testInstance.ApplyFilters();
            results.Should().OnlyContain(result => result.String.ToLowerInvariant() == "cheese");
        }
    }
}
