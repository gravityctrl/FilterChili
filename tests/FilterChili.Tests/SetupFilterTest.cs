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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using GravityCTRL.FilterChili.Tests.Shared.Contexts;
using GravityCTRL.FilterChili.Tests.Shared.Models;
using GravityCTRL.FilterChili.Tests.Shared.Services;
using GravityCTRL.FilterChili.Tests.Utils;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;
using static GravityCTRL.FilterChili.Tests.Shared.Utils.Benchmark;
using static GravityCTRL.FilterChili.Tests.Shared.Utils.JsonUtils;

namespace GravityCTRL.FilterChili.Tests
{
    [UsedImplicitly]
    public class DatabaseFixture : IDisposable
    {
        private const int ENTITY_AMOUNT = 100_000;
        private readonly DataContext _context;

        public ProductService Service { get; }

        public DatabaseFixture()
        {
            _context = DataContext.CreateWithSqlServer(Guid.NewGuid().ToString());
            _context.Migrate();

            var products = CreateTestProducts();

            var service = new ProductService(_context);
            service.AddRange(products.ToList()).Wait();

            Service = service;
        }

        public void Dispose()
        {
            _context.Delete();
            _context.Dispose();
        }

        private static IEnumerable<Product> CreateTestProducts()
        {
            Randomizer.Seed = new Random(0);

            var testProducts = new Faker<Product>();
            testProducts.RuleFor(product => product.Sold, faker => faker.Random.Int(0, 1000));
            testProducts.RuleFor(product => product.Rating, faker => faker.Random.Int(1, 10));
            testProducts.RuleFor(product => product.Name, faker => faker.Commerce.Product());
            return testProducts.GenerateLazy(ENTITY_AMOUNT);
        }
    }

    public class SetupFilterTest : IClassFixture<DatabaseFixture>
    {
        private const int FILTER_ASSIGNMENTS = 100_000;
        private const int MAX_PRINTED_RESULTS = 5;

        private readonly ITestOutputHelper _output;
        private readonly JObject _rangeObject;
        private readonly JObject _listObject;
        private readonly JArray _allArrayObject;
        private readonly ProductService _service;

        public SetupFilterTest(DatabaseFixture fixture, ITestOutputHelper output)
        {
            _service = fixture.Service;
            _output = output;

            var rangeJson = ResourceHelper.Load("rangefilter.json");
            _rangeObject = JObject.Parse(rangeJson);

            var listJson = ResourceHelper.Load("listfilter.json");
            _listObject = JObject.Parse(listJson);

            var allFiltersJson = ResourceHelper.Load("allfilters.json");
            _allArrayObject = JArray.Parse(allFiltersJson);
        }

        [Fact]
        public async Task Should_Set_Filter_With_Resolver_Instance()
        {
            var filterContext = new ProductFilterContext(_service.Entities);

            var duration = await Measure(async () =>
            {
                for (var i = 0; i < FILTER_ASSIGNMENTS; i++)
                {
                    filterContext.RatingFilter.Set(1, 7);
                    filterContext.NameFilter.Set("Piza", "Chicken", "Chese", "Fish", "Tun");
                }

                await PerformAnalysis(filterContext);
            });

            _output.WriteLine("Duration {0}", duration);
        }

        [Fact]
        public async Task Should_Set_Filter_With_TrySet()
        {
            var filterContext = new ProductFilterContext(_service.Entities);

            var duration = await Measure(async () =>
            {
                for (var i = 0; i < FILTER_ASSIGNMENTS; i++)
                {
                    filterContext.TrySet("Rating", 1, 7);
                    filterContext.TrySet("Name", new[] { "Piza", "Chicken", "Chese", "Fish", "Tun" });
                }

                await PerformAnalysis(filterContext);
            });

            _output.WriteLine("Duration {0}", duration);
        }

        [Fact]
        public async Task Should_Set_Filter_With_TrySet_Json()
        {
            var filterContext = new ProductFilterContext(_service.Entities);

            var duration = await Measure(async () =>
            {
                for (var i = 0; i < FILTER_ASSIGNMENTS; i++)
                {
                    filterContext.TrySet(_rangeObject);
                    filterContext.TrySet(_listObject);
                }

                await PerformAnalysis(filterContext);
            });

            _output.WriteLine("Duration {0}", duration);
        }

        [Fact]
        public async Task Should_Set_Filter_With_TrySet_JsonArray()
        {
            var filterContext = new ProductFilterContext(_service.Entities);

            var duration = await Measure(async () =>
            {
                for (var i = 0; i < FILTER_ASSIGNMENTS; i++)
                {
                    filterContext.TrySet(_allArrayObject);
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
