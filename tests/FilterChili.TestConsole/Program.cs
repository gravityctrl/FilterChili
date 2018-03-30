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
using GravityCTRL.FilterChili.Tests.Shared.Utils;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace GravityCTRL.FilterChili.TestConsole
{
    // ReSharper disable once ClassNeverInstantiated.Global
    // ReSharper disable once MemberCanBeInternal
    public class Program
    {
        private const int MAX_PRINTED_RESULTS = 10;
        private const int ENTITY_AMOUNT = 10_000;

        public static void Main()
        {
            using (var dataContext = DataContext.CreateInMemory(Guid.NewGuid().ToString()))
            {
                dataContext.Migrate();

                var service = new ProductService(dataContext);
                service.AddRange(CreateTestProducts()).Wait();

                string input = null;
                do
                {
                    Console.Clear();
                    if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var filterContext = new ProductFilterContext(service.Entities);

                    var duration1 = Benchmark.Measure(() =>
                    {
                        // ReSharper disable once AccessToModifiedClosure
                        filterContext.SetSearch(input);
                        filterContext.TrySet("Rating", 1, 7);
                        filterContext.TrySet("Name", new[] { "Piza", "Chicken", "Chese", "Fish", "Tun" });
                        filterContext.TrySet("Sold", 600);
                    });

                    var duration2 = Benchmark.Measure(() => {
                        PerformResultAnalysis(filterContext).Wait();
                    });

                    var duration3 = Benchmark.Measure(() =>
                    {
                        PerformFilterAnalysis(filterContext).Wait();
                    });

                    Console.WriteLine("Duration {0}", duration1);
                    Console.WriteLine("Duration {0}", duration2);
                    Console.WriteLine("Duration {0}", duration3);

                    Console.Write($"{Environment.NewLine}ENTER SEARCH TERMS: ");
                    input = Console.ReadLine()?.ToLowerInvariant();
                }
                while (!string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase));

                dataContext.Delete();
            }
        }

        private static IEnumerable<Product> CreateTestProducts()
        {
            Randomizer.Seed = new Random(0);

            var testProducts = new Faker<Product>();
            testProducts.RuleFor(product => product.Sold, faker => faker.Random.Int(0, 1000));
            testProducts.RuleFor(product => product.Rating, faker => faker.Random.Int(1, 10));
            testProducts.RuleFor(product => product.Name, faker => faker.Commerce.Product());
            testProducts.RuleFor(product => product.Category, faker => faker.Commerce.ProductMaterial());
            return testProducts.GenerateLazy(ENTITY_AMOUNT);
        }

        private static async Task PerformResultAnalysis([NotNull] ProductFilterContext context)
        {
            var filterResults = context.ApplyFilters().Take(MAX_PRINTED_RESULTS);
            var evaluatedFilterResults = await filterResults.ToListAsync();
            Console.WriteLine(JsonUtils.Convert(evaluatedFilterResults));
        }

        private static async Task PerformFilterAnalysis([NotNull] ProductFilterContext context)
        {
            var domains = await context.Domains();
            Console.WriteLine(JsonUtils.Convert(domains));
        }
    }
}
