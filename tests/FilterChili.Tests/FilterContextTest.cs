﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using GravityCTRL.FilterChili.Enums;
using GravityCTRL.FilterChili.Tests.Shared.Contexts;
using GravityCTRL.FilterChili.Tests.Shared.Models;
using GravityCTRL.FilterChili.Tests.TestSupport.Utils;
using Newtonsoft.Json.Linq;
using Xunit;

namespace GravityCTRL.FilterChili.Tests
{
    public class FilterContextTest
    {
        private readonly JObject _rangeObject;
        private readonly JObject _listObject;
        private readonly JObject _greaterThanObject;
        private readonly JArray _allArrayObject;
        private readonly JObject _invalidFilterObject;
        private readonly JObject _notExistingFilterObject;
        private readonly string[] _allowedNames;

        public FilterContextTest()
        {
            var rangeJson = ResourceHelper.Load("rangefilter.json");
            _rangeObject = JObject.Parse(rangeJson);

            var listJson = ResourceHelper.Load("listfilter.json");
            _listObject = JObject.Parse(listJson);

            var greaterThanJson = ResourceHelper.Load("greaterthanfilter.json");
            _greaterThanObject = JObject.Parse(greaterThanJson);

            var allFiltersJson = ResourceHelper.Load("allfilters.json");
            _allArrayObject = JArray.Parse(allFiltersJson);

            var invalidFilterJson = ResourceHelper.Load("invalidfilter.json");
            _invalidFilterObject = JObject.Parse(invalidFilterJson);

            var notExistingFilterJson = ResourceHelper.Load("notexistingfilter.json");
            _notExistingFilterObject = JObject.Parse(notExistingFilterJson);

            _allowedNames = new[] { "Piza", "Chicken", "Chese", "Fish", "Tun" };
        }

        [Fact]
        public async Task Should_Set_Filter_With_Resolver_Instance()
        {
            var filterContext = new ProductFilterContext(CreateTestProducts().AsQueryable());

            filterContext.TrySet("Rating", 1, 7);
            filterContext.TrySet("Name", _allowedNames);
            filterContext.TrySet("Sold", 600);

            var filterResults = filterContext.ApplyFilters();
            filterResults.Should().NotContain(entity => entity.Rating < 1 || entity.Rating > 7);
            filterResults.Should().NotContain(entity => !_allowedNames.Contains(entity.Name));
            filterResults.Should().NotContain(entity => entity.Sold <= 600);

            var domains = await filterContext.Domains();
            domains.Should().HaveCount(3);
        }

        [Fact]
        public async Task Should_Set_Filter_With_TrySet()
        {
            var filterContext = new ProductFilterContext(CreateTestProducts().AsQueryable());

            filterContext.RatingFilter.Set(1, 7);
            filterContext.NameFilter.Set("Piza", "Chicken", "Chese", "Fish", "Tun");
            filterContext.SoldFilter.Set(600);

            var filterResults = filterContext.ApplyFilters();
            filterResults.Should().NotContain(entity => entity.Rating < 1 || entity.Rating > 7);
            filterResults.Should().NotContain(entity => !_allowedNames.Contains(entity.Name));
            filterResults.Should().NotContain(entity => entity.Sold <= 600);

            var domains = await filterContext.Domains(CalculationStrategy.WithoutSelectableValues);
            domains.Should().HaveCount(3);
        }

        [Fact]
        public async Task Should_Set_Filter_With_TrySet_Json()
        {
            var filterContext = new ProductFilterContext(CreateTestProducts().AsQueryable());

            filterContext.TrySet(_rangeObject);
            filterContext.TrySet(_listObject);
            filterContext.TrySet(_greaterThanObject);

            var filterResults = filterContext.ApplyFilters();
            filterResults.Should().NotContain(entity => entity.Rating < 1 || entity.Rating > 7);
            filterResults.Should().NotContain(entity => !_allowedNames.Contains(entity.Name));
            filterResults.Should().NotContain(entity => entity.Sold <= 600);

            var domains = await filterContext.Domains();
            domains.Should().HaveCount(3);
        }

        [Fact]
        public async Task Should_Set_Filter_With_TrySet_JsonArray()
        {
            var filterContext = new ProductFilterContext(CreateTestProducts().AsQueryable());

            filterContext.TrySet(_allArrayObject);

            var filterResults = filterContext.ApplyFilters();
            filterResults.Should().NotContain(entity => entity.Rating < 1 || entity.Rating > 7);
            filterResults.Should().NotContain(entity => !_allowedNames.Contains(entity.Name));
            filterResults.Should().NotContain(entity => entity.Sold <= 600);

            var domains = await filterContext.Domains(CalculationStrategy.WithoutSelectableValues);
            domains.Should().HaveCount(3);
        }

        [Fact]
        public void Should_Not_Be_Able_To_Set_Invalid_Filter()
        {
            var filterContext = new ProductFilterContext(CreateTestProducts().AsQueryable());
            filterContext.TrySet(_invalidFilterObject).Should().BeFalse();
        }

        [Fact]
        public void Should_Not_Be_Able_To_Set_Filter_With_Wrong_Name()
        {
            var filterContext = new ProductFilterContext(CreateTestProducts().AsQueryable());
            filterContext.TrySet(_notExistingFilterObject).Should().BeFalse();
        }

        private static List<Product> CreateTestProducts()
        {
            Randomizer.Seed = new Random(0);

            var testProducts = new Faker<Product>();
            testProducts.RuleFor(product => product.Sold, faker => faker.Random.Int(0, 1000));
            testProducts.RuleFor(product => product.Rating, faker => faker.Random.Int(1, 10));
            testProducts.RuleFor(product => product.Name, faker => faker.Commerce.Product());
            return testProducts.GenerateLazy(100).ToList();
        }
    }
}
