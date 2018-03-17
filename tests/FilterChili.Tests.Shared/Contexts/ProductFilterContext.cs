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

using System.Linq;
using GravityCTRL.FilterChili.Tests.Shared.Models;
using JetBrains.Annotations;

namespace GravityCTRL.FilterChili.Tests.Shared.Contexts
{
    public class ProductFilterContext : FilterContext<Product>
    {
        [UsedImplicitly]
        public ListResolver<Product, string> NameFilter { get; set; }

        [UsedImplicitly]
        public ComparisonResolver<Product, int> SoldFilter { get; set; }

        [UsedImplicitly]
        public RangeResolver<Product, int> RatingFilter { get; set; }

        public ProductFilterContext(IQueryable<Product> queryable) : base(queryable) {}

        protected override void Configure(ContextOptions<Product> options)
        {
            options.CalculationStrategy = CalculationStrategy.Full;

            NameFilter = options.Filter(product => product.Name).WithList().UseName("Name");
            RatingFilter = options.Filter(product => product.Rating).WithRange();
            SoldFilter = options.Filter(product => product.Sold).WithGreaterThan();
        }
    }
}
