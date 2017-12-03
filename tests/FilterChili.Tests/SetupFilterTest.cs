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
using System.Threading.Tasks;
using GravityCTRL.FilterChili.Tests.Models;
using GravityCTRL.FilterChili.Tests.Utils;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace GravityCTRL.FilterChili.Tests
{
    public class SetupFilterTest
    {
        private readonly ITestOutputHelper _output;
        private readonly JObject _rangeObject;
        private readonly JObject _listObject;

        public SetupFilterTest(ITestOutputHelper output)
        {
            _output = output;

            var rangeJson = ResourceHelper.Load("rangefilter.json");
            _rangeObject = JObject.Parse(rangeJson);

            var listJson = ResourceHelper.Load("listfilter.json");
            _listObject = JObject.Parse(listJson);
        }

        [Fact]
        public async Task Should_Set_Filter_With_Resolver_Instance()
        {
            var context = CreateContext();
            for (var i = 0; i < 1_000_000; i++)
            {
                context.RatingFilter.Set(1, 7);
                context.NameFilter.Set("Test2");
            }

            var filterResults = context.ApplyFilters();
            var evaluatedFilterResults = filterResults.ToList();

            var domains = await context.Domains();
            _output.WriteLine(JsonUtils.Convert(domains));
            _output.WriteLine(JsonUtils.Convert(evaluatedFilterResults));
        }

        [Fact]
        public async Task Should_Set_Filter_With_TrySet()
        {
            var context = CreateContext();
            for (var i = 0; i < 1_000_000; i++)
            {
                context.TrySet("Rating", 1, 7);
                context.TrySet("Name", new[] { "Test2" });
            }

            var filterResults = context.ApplyFilters();
            var evaluatedFilterResults = filterResults.ToList();

            var domains = await context.Domains();
            _output.WriteLine(JsonUtils.Convert(domains));
            _output.WriteLine(JsonUtils.Convert(evaluatedFilterResults));
        }

        [Fact]
        public async Task Should_Set_Filter_With_TrySet_Json()
        {
            var context = CreateContext();
            for (var i = 0; i < 1_000_000; i++)
            {
                context.TrySet(_rangeObject);
                context.TrySet(_listObject);
            }

            var filterResults = context.ApplyFilters();
            var evaluatedFilterResults = filterResults.ToList();

            var domains = await context.Domains();
            _output.WriteLine(JsonUtils.Convert(domains));
            _output.WriteLine(JsonUtils.Convert(evaluatedFilterResults));
        }

        private static ProductFilterContext CreateContext()
        {
            var products = new List<Product>
            {
                new Product
                {
                    Sold = 1,
                    Rating = 2,
                    Name = "Test1"
                },
                new Product
                {
                    Sold = 2,
                    Rating = 6,
                    Name = "Test2"
                },
                new Product
                {
                    Sold = 5,
                    Rating = 9,
                    Name = "Test2"
                }
            };

            var queryable = products.AsQueryable();
            return new ProductFilterContext(queryable);
        }
    }
}
