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
using Bogus;
using GravityCTRL.FilterChili.Tests.Shared.Contexts;
using GravityCTRL.FilterChili.Tests.Shared.Models;
using JetBrains.Annotations;

namespace GravityCTRL.FilterChili.Tests.TestSupport.TestFixtures
{
    [UsedImplicitly]
    public class DatabaseFixture : IDisposable
    {
        private const int ENTITY_AMOUNT = 100_000;
        private readonly DataContext _context;

        public DatabaseFixture()
        {
            _context = DataContext.CreateInMemory(Guid.NewGuid().ToString());
            _context.Migrate();

            var products = CreateTestProducts();

            _context.Products.AddRangeAsync(products.ToList());
            _context.SaveChanges();
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
}