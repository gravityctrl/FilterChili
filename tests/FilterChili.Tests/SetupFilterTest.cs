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
using GravityCTRL.FilterChili.Tests.Contexts;
using GravityCTRL.FilterChili.Tests.Models;
using GravityCTRL.FilterChili.Tests.Services;
using GravityCTRL.FilterChili.Tests.Utils;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;
using static GravityCTRL.FilterChili.Tests.Utils.JsonUtils;
using static GravityCTRL.FilterChili.Tests.Utils.Benchmark;

namespace GravityCTRL.FilterChili.Tests
{
    [UsedImplicitly]
    public class DatabaseFixture : IDisposable
    {
        private const int ENTITY_AMOUNT = 100_000;
        private readonly TestContext _context;

        public ProductService Service { get; }

        public DatabaseFixture()
        {
            _context = TestContext.CreateInstance();
            var service = new ProductService(_context);

            if (service.Any().Result)
            {
                return;
            }

            Randomizer.Seed = new Random(0);

            var index = 1;
            var testProducts = new Faker<Product>();
            testProducts.RuleFor(product => product.Id, faker => index++);
            testProducts.RuleFor(product => product.Sold, faker => faker.Random.Int(0, 1000));
            testProducts.RuleFor(product => product.Rating, faker => faker.Random.Int(1, 10));
            testProducts.RuleFor(product => product.Name, faker => faker.Commerce.Product());
            var products = testProducts.GenerateLazy(ENTITY_AMOUNT);

            service.AddRange(products).Wait();

            Service = service;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }

    public class SetupFilterTest : IClassFixture<DatabaseFixture>
    {
        private const int FILTER_ASSIGNMENTS = 100_000;

        private const int MAX_PRINTED_RESULTS = 5;

        private readonly ITestOutputHelper _output;
        private readonly JObject _rangeObject;
        private readonly JObject _listObject;
        private readonly ProductService _service;

        public SetupFilterTest(DatabaseFixture fixture, ITestOutputHelper output)
        {
            _service = fixture.Service;
            _output = output;

            var rangeJson = ResourceHelper.Load("rangefilter.json");
            _rangeObject = JObject.Parse(rangeJson);

            var listJson = ResourceHelper.Load("listfilter.json");
            _listObject = JObject.Parse(listJson);
        }

        [Fact]
        public async Task Should_Set_Filter_With_Resolver_Instance()
        {
            var duration = await Measure(async () =>
            {
                var filterContext = new ProductFilterContext(_service.Entities);

                for (var i = 0; i < FILTER_ASSIGNMENTS; i++)
                {
                    filterContext.RatingFilter.Set(1, 7);
                    filterContext.NameFilter.Set("Bizza", "Chicken", "Chese", "Fish", "Tun");
                }

                await PerformAnalysis(filterContext);
            });

            _output.WriteLine("Duration {0}", duration);
        }

        [Fact]
        public async Task Should_Set_Filter_With_TrySet()
        {
            var duration = await Measure(async () =>
            {
                var filterContext = new ProductFilterContext(_service.Entities);

                for (var i = 0; i < FILTER_ASSIGNMENTS; i++)
                {
                    filterContext.TrySet("Rating", 1, 7);
                    filterContext.TrySet("Name", new[] {"Bizza", "Chicken", "Chese", "Fish", "Tun"});
                }

                await PerformAnalysis(filterContext);
            });

            _output.WriteLine("Duration {0}", duration);
        }

        [Fact]
        public async Task Should_Set_Filter_With_TrySet_Json()
        {
            var duration = await Measure(async () =>
            {
                var filterContext = new ProductFilterContext(_service.Entities);

                for (var i = 0; i < FILTER_ASSIGNMENTS; i++)
                {
                    filterContext.TrySet(_rangeObject);
                    filterContext.TrySet(_listObject);
                }

                await PerformAnalysis(filterContext);
            });

            _output.WriteLine("Duration {0}", duration);
        }

        private async Task PerformAnalysis(ProductFilterContext context)
        {
            var filterResults = context.ApplyFilters().Take(MAX_PRINTED_RESULTS);
            var evaluatedFilterResults = filterResults.ToList();
            _output.WriteLine(Convert(evaluatedFilterResults));

            var domains = await context.Domains();
            _output.WriteLine(Convert(domains));
        }
    }
}
