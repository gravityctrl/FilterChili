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

using GravityCTRL.FilterChili.Tests.Models;
using Microsoft.EntityFrameworkCore;

namespace GravityCTRL.FilterChili.Tests.Contexts
{
    public class TestContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        private TestContext(DbContextOptions<TestContext> options) : base(options) {}

        public static TestContext CreateInstance()
        {
            var options = new DbContextOptionsBuilder<TestContext>().UseInMemoryDatabase("database").Options;
            return new TestContext(options);
        }
    }
}
