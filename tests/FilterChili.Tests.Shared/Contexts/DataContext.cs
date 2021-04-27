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

using GravityCTRL.FilterChili.Tests.Shared.Models;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GravityCTRL.FilterChili.Tests.Shared.Contexts
{
    public sealed class DataContext : DbContext
    {
        private static readonly ILoggerFactory Factory;

        [UsedImplicitly]
        private const string LOCALDB = "(localdb)\\mssqllocaldb";

        [UsedImplicitly]
        private const string SQLEXPRESS = "localhost\\sqlexpress";

        internal DbSet<Product> Products { get; [UsedImplicitly] set; }

        static DataContext()
        {
            Factory = LoggerFactory.Create(opt => opt.AddConsole());
        }

        private DataContext([NotNull] DbContextOptions options) : base(options) {}

        [NotNull]
        [UsedImplicitly]
        public static DataContext CreateWithSqlServer([NotNull] string databaseName)
        {
            var builder = new DbContextOptionsBuilder<DataContext>();
            var options = builder.UseSqlServer($"Server={SQLEXPRESS};Database={databaseName};Trusted_Connection=True;MultipleActiveResultSets=true").UseLoggerFactory(Factory).Options;
            return new DataContext(options);
        }

        [NotNull]
        [UsedImplicitly]
        public static DataContext CreateInMemory([NotNull] string databaseName)
        {
            var builder = new DbContextOptionsBuilder<DataContext>();
            var options = builder.UseInMemoryDatabase(databaseName).Options;
            return new DataContext(options);
        }

        protected override void OnModelCreating([NotNull] ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasKey(p => p.Id);
            modelBuilder.Entity<Product>().Property(p => p.Name).HasMaxLength(70);
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
