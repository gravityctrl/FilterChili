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
    public sealed class ProductFilterContext : FilterContext<Product>
    {
        [UsedImplicitly]
        public SearchSpecification<Product> NameSearch { get; set; }

        [UsedImplicitly]
        public SearchSpecification<Product> CategorySearch { get; set; }

        [UsedImplicitly]
        public SearchSpecification<Product> IdSearch { get; set; }

        [UsedImplicitly]
        public SearchSpecification<Product> RatingSearch { get; set; }

        [UsedImplicitly]
        public SearchSpecification<Product> SoldSearch { get; set; }

        [UsedImplicitly]
        public GroupResolver<Product, string, string> NameFilter { get; set; }

        [UsedImplicitly]
        public ComparisonResolver<Product, int> SoldFilter { get; set; }

        [UsedImplicitly]
        public RangeResolver<Product, int> RatingFilter { get; set; }

        public ProductFilterContext(IQueryable<Product> queryable) : base(queryable) {}

        protected override void Configure(ContextOptions<Product> options)
        {
            NameSearch = options.Search(product => product.Name);
            CategorySearch = options.Search(product => product.Category);
            IdSearch = options.Search(product => product.Id.ToString()).UseEquals();
            RatingSearch = options.Search(product => product.Rating.ToString()).UseEquals();
            SoldSearch = options.Search(product => product.Sold.ToString()).UseEquals();

            NameFilter = options.Filter(product => product.Name).WithGroup(product => product.Category).UseDefaultGroup("Unknown");
            RatingFilter = options.Filter(product => product.Rating).WithRange();
            SoldFilter = options.Filter(product => product.Sold).WithGreaterThan();
        }
    }
}
