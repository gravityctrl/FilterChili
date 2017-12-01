﻿// This file is part of FilterChili.
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
using FluentAssertions;
using GravityCTRL.FilterChili.Tests.Models;
using GravityCTRL.FilterChili.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace GravityCTRL.FilterChili.Tests
{
    public class SetupFilterTest
    {
        private readonly ITestOutputHelper _output;

        public SetupFilterTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Should_Set_Filter_With_Resolver_Instance()
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
            var context = new ProductFilterContext(queryable);

            context.RatingFilter.Set(1, 7);
            context.NameFilter.Set("Test2");

            var filterResults = context.ApplyFilters();
            var evaluatedFilterResults = filterResults.ToList();

            _output.WriteLine(JsonUtils.Convert(context.Domains()));
            _output.WriteLine(JsonUtils.Convert(evaluatedFilterResults));
        }

        [Fact]
        public void Should_Set_Filter_With_TrySet()
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
            var context = new ProductFilterContext(queryable);

            context.TrySet("Rating", 1, 7);
            context.TrySet("Name", "Test2");

            var filterResults = context.ApplyFilters();
            var evaluatedFilterResults = filterResults.ToList();

            _output.WriteLine(JsonUtils.Convert(context.Domains()));
            _output.WriteLine(JsonUtils.Convert(evaluatedFilterResults));
        }
    }
}
