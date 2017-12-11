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

using System;
using GravityCTRL.FilterChili.Tests.Models;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace GravityCTRL.FilterChili.Tests.Contexts
{
    public class TestContext : DbContext
    {
        public DbSet<Product> Products { get; [UsedImplicitly] set; }

        private TestContext(DbContextOptions options) : base(options) {}

        public static TestContext CreateWithSqlite(string databaseName)
        {
            var builder = new DbContextOptionsBuilder<TestContext>();
            var options = builder.UseSqlServer($"Server=(localdb)\\mssqllocaldb;Database={databaseName};Trusted_Connection=True;MultipleActiveResultSets=true").Options;
            return new TestContext(options);
        }

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
