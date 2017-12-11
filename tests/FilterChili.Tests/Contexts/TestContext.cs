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
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace GravityCTRL.FilterChili.Tests.Contexts
{
    public class TestContext : DbContext
    {
        [UsedImplicitly]
        private const string LOCALDB = "(localdb)\\mssqllocaldb";

        [UsedImplicitly]
        private const string SQLEXPRESS = "localhost\\sqlexpress";

        public DbSet<Product> Products { get; [UsedImplicitly] set; }

        private TestContext(DbContextOptions options) : base(options) {}

        [UsedImplicitly]
        public static TestContext CreateWithSqlServer(string databaseName)
        {
            var builder = new DbContextOptionsBuilder<TestContext>();
            var options = builder.UseSqlServer($"Server={SQLEXPRESS};Database={databaseName};Trusted_Connection=True;MultipleActiveResultSets=true").Options;
            return new TestContext(options);
        }

        [UsedImplicitly]
        public static TestContext CreateInMemory(string databaseName)
        {
            var builder = new DbContextOptionsBuilder<TestContext>();
            var options = builder.UseInMemoryDatabase(databaseName).Options;
            return new TestContext(options);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasKey(p => p.Id);
            base.OnModelCreating(modelBuilder);
        }

        public void Migrate()
        {
            if (!Database.IsSqlServer())
            {
                return;
            }

            Database.EnsureCreated();
            Database.Migrate();
        }

        public void Delete()
        {
            if (Database.IsSqlServer())
            {
                Database.EnsureDeleted();
            }
        }
    }
}
