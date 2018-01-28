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
using System.Linq.Expressions;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using GravityCTRL.FilterChili.Enums;
using GravityCTRL.FilterChili.Exceptions;
using GravityCTRL.FilterChili.Resolvers;
using GravityCTRL.FilterChili.Selectors;
using GravityCTRL.FilterChili.Tests.TestSupport.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace GravityCTRL.FilterChili.Tests
{
    public class ContextOptionsTest
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
            _testInstance.CalculationStrategy.Should().Be(CalculationStrategy.Full);
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
            Action enumerateAction = () => domains.ToHashSet();
            enumerateAction.ShouldThrow<MissingResolverException>();
        }

        [Fact]
        public async Task Should_Be_Able_To_Use_FilterSelectors_With_Resolver()
        {
            _testInstance.Filter(source => source.Byte).With(domain => new TestDomainResolver<byte>("Byte", domain.Selector));
            _testInstance.Filter(source => source.Char).With(domain => new TestDomainResolver<char>("Char", domain.Selector));
            _testInstance.Filter(source => source.Decimal).With(domain => new TestDomainResolver<decimal>("Decimal", domain.Selector));
            _testInstance.Filter(source => source.Double).With(domain => new TestDomainResolver<double>("Double", domain.Selector));
            _testInstance.Filter(source => source.Float).With(domain => new TestDomainResolver<float>("Float", domain.Selector));
            _testInstance.Filter(source => source.Int).With(domain => new TestDomainResolver<int>("Int", domain.Selector));
            _testInstance.Filter(source => source.Long).With(domain => new TestDomainResolver<long>("Long", domain.Selector));
            _testInstance.Filter(source => source.SByte).With(domain => new TestDomainResolver<sbyte>("SByte", domain.Selector));
            _testInstance.Filter(source => source.Short).With(domain => new TestDomainResolver<short>("Short", domain.Selector));
            _testInstance.Filter(source => source.String).With(domain => new TestDomainResolver<string>("String", domain.Selector));
            _testInstance.Filter(source => source.UInt).With(domain => new TestDomainResolver<uint>("UInt", domain.Selector));
            _testInstance.Filter(source => source.ULong).With(domain => new TestDomainResolver<ulong>("ULong", domain.Selector));
            _testInstance.Filter(source => source.UShort).With(domain => new TestDomainResolver<ushort>("UShort", domain.Selector));

            var domains = await _testInstance.Domains();
            domains.Should().HaveCount(13);
        }

        [Fact]
        public async Task Should_Not_Set_Values_If_NeedsToBeResolved_Is_False()
        {
            _testInstance.Filter(source => source.Byte).With(domain => new TestDomainResolver<byte>("Byte", domain.Selector));
            _testInstance.Filter(source => source.Double).With(domain => new TestDomainResolver<double>("Double", domain.Selector));
            _testInstance.Filter(source => source.String).With(domain => new TestDomainResolver<string>("String", domain.Selector));
            _testInstance.CalculationStrategy = CalculationStrategy.WithoutSelectableValues;

            var domains = await _testInstance.Domains();

            foreach (var domain in domains.Cast<ITestDomainResolver>())
            {
                domain.SetAvailableValuesCallCount.Should().Be(0);
                domain.SetSelectableValuesCallCount.Should().Be(0);
            }
        }

        [Fact]
        public async Task Should_Set_AvailableValues_If_NeedsToBeResolved_Is_True_And_CalculationStrategy_Ignores_SelectableValues()
        {
            _testInstance.Filter(source => source.Byte).With(domain => new TestDomainResolver<byte>("Byte", domain.Selector) { NeedsToBeResolved = true });
            _testInstance.Filter(source => source.Double).With(domain => new TestDomainResolver<double>("Double", domain.Selector) { NeedsToBeResolved = true });
            _testInstance.Filter(source => source.String).With(domain => new TestDomainResolver<string>("String", domain.Selector) { NeedsToBeResolved = true });
            _testInstance.CalculationStrategy = CalculationStrategy.WithoutSelectableValues;

            var domains = await _testInstance.Domains();

            foreach (var domain in domains.Cast<ITestDomainResolver>())
            {
                domain.SetAvailableValuesCallCount.Should().Be(1);
                domain.SetSelectableValuesCallCount.Should().Be(0);
            }
        }

        [Fact]
        public async Task Should_Set_All_Values_If_NeedsToBeResolved_Is_True_And_CalculationStrategy_Is_Full()
        {
            _testInstance.Filter(source => source.Byte).With(domain => new TestDomainResolver<byte>("Byte", domain.Selector) { NeedsToBeResolved = true });
            _testInstance.Filter(source => source.Double).With(domain => new TestDomainResolver<double>("Double", domain.Selector) { NeedsToBeResolved = true });
            _testInstance.Filter(source => source.String).With(domain => new TestDomainResolver<string>("String", domain.Selector) { NeedsToBeResolved = true });
            _testInstance.CalculationStrategy = CalculationStrategy.Full;

            var domains = (await _testInstance.Domains()).ToList();

            foreach (var domain in domains.Cast<ITestDomainResolver>())
            {
                domain.SetAvailableValuesCallCount.Should().Be(1);
                domain.SetSelectableValuesCallCount.Should().Be(1);
            }
        }

        [Fact]
        public async Task Should_Resolve_Domains_Independent_From_Setting_Filters_Order()
        {
            var filter1 = _testInstance.Filter(source => source.Int).With(domain => domain.GreaterThan("Int"));
            var filter2 = _testInstance.Filter(source => source.Double).With(domain => domain.LessThanOrEqual("Double"));
            var filter3 = _testInstance.Filter(source => source.Float).With(domain => domain.Range("Float"));
            _testInstance.CalculationStrategy = CalculationStrategy.Full;

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
            _testInstance.Filter(source => source.Int).With(domain => domain.GreaterThan("Int"));
            _testInstance.Filter(source => source.Double).With(domain => domain.LessThanOrEqual("Double"));
            _testInstance.Filter(source => source.Float).With(domain => domain.Range("Float"));

            _testInstance.GetFilter("Int").Domain().Name.Should().Be("Int");
            _testInstance.GetFilter("Double").Domain().Name.Should().Be("Double");
            _testInstance.GetFilter("Float").Domain().Name.Should().Be("Float");
            _testInstance.GetFilter("Byte").Should().BeNull();
        }

        [Fact]
        public void Should_Return_Expected_Entities_When_Applying_Filters()
        {
            var filter1 = _testInstance.Filter(source => source.Int).With(domain => domain.GreaterThan("Int"));
            var filter2 = _testInstance.Filter(source => source.Double).With(domain => domain.LessThanOrEqual("Double"));
            var filter3 = _testInstance.Filter(source => source.Float).With(domain => domain.Range("Float"));

            filter1.Set(3);
            filter2.Set(15);
            filter3.Set(5, 25);

            var results = _testInstance.ApplyFilters().ToList();
            results.Should().NotContain(entity => entity.Int <= 3);
            results.Should().NotContain(entity => entity.Double > 15);
            results.Should().NotContain(entity => entity.Float < 5 && entity.Float > 25);
        }

        private interface ITestDomainResolver
        {
            int SetAvailableValuesCallCount { get; }
            int SetSelectableValuesCallCount { get; }
        }

        private class TestDomainResolver<T> : DomainResolver<GenericSource, T>, ITestDomainResolver where T : IComparable
        {
            internal override bool NeedsToBeResolved { get; set; }
            public override string FilterType { get; }

            public int SetAvailableValuesCallCount { get; private set; }
            public int SetSelectableValuesCallCount { get; private set; }

            internal TestDomainResolver(string name, Expression<Func<GenericSource, T>> selector) : base(name, selector)
            {
                FilterType = "TestType";
            }

            public override bool TrySet(JToken domainToken)
            {
                return true;
            }

            protected override async Task SetAvailableValues(IQueryable<T> allValues)
            {
                SetAvailableValuesCallCount++;
                await Task.FromResult(0);
            }

            protected override async Task SetSelectableValues(IQueryable<T> selectableItems)
            {
                SetSelectableValuesCallCount++;
                await Task.FromResult(0);
            }

            protected override Expression<Func<GenericSource, bool>> FilterExpression()
            {
                return _ => true;
            }
        }
    }
}
