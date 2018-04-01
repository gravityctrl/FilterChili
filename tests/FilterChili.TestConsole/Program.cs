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
using GravityCTRL.FilterChili.Resolvers;
using GravityCTRL.FilterChili.Tests.Shared.Contexts;
using GravityCTRL.FilterChili.Tests.Shared.Models;
using GravityCTRL.FilterChili.Tests.Shared.Services;
using GravityCTRL.FilterChili.Tests.Shared.Utils;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace GravityCTRL.FilterChili.TestConsole
{
    [UsedImplicitly]
    public static class Program
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

                string readline = null;
                do
                {
                    var input = readline;
                    Console.Clear();
                    if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var filterContext = new ProductFilterContext(service.Entities);

                    var benchmarkResult1 = Benchmark.Measure(() =>
                    {
                        filterContext.SetSearch(input);
                        filterContext.TrySet("Rating", 1, 7);
                        filterContext.TrySet("Name", new[] { "Piza", "Chicken", "Chese", "Fish", "Tun" });
                        filterContext.TrySet("Sold", 600);
                    });

                    // ReSharper disable once ImplicitlyCapturedClosure
                    var benchmarkResult2 = Benchmark.Measure(() => PerformResultAnalysis(filterContext).Result);

                    // ReSharper disable once ImplicitlyCapturedClosure
                    var benchmarkResult3 = Benchmark.Measure(() => PerformFilterAnalysis(filterContext).Result);

                    Console.WriteLine(JsonUtils.Convert(benchmarkResult2.Result));
                    Console.WriteLine(JsonUtils.Convert(benchmarkResult3.Result));

                    Console.WriteLine("Duration {0}", benchmarkResult1.ElapsedTime);
                    Console.WriteLine("Duration {0}", benchmarkResult2.ElapsedTime);
                    Console.WriteLine("Duration {0}", benchmarkResult3.ElapsedTime);

                    Console.Write($"{Environment.NewLine}ENTER SEARCH TERMS: ");
                    readline = Console.ReadLine()?.ToLowerInvariant();
                }
                while (!string.Equals(readline, "exit", StringComparison.OrdinalIgnoreCase));

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

        private static async Task<List<Product>> PerformResultAnalysis([NotNull] ProductFilterContext context)
        {
            var filterResults = context.ApplyFilters().Take(MAX_PRINTED_RESULTS);
            return await filterResults.ToListAsync();
        }

        [ItemNotNull]
        private static async Task<IEnumerable<FilterResolver<Product>>> PerformFilterAnalysis([NotNull] ProductFilterContext context)
        {
            return await context.Domains();
        }
    }
}
