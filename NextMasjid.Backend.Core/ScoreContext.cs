﻿using System.IO;
using Microsoft.EntityFrameworkCore;
namespace NextMasjid.Backend.Core
{
    public class ScoreContext : DbContext
    {
        public bool IsInMemory { get; set; }
        public DbSet<ScoreModelDb> Scores { get; set; }

        public string DbPath { get; set; }

        public ScoreContext()
        {

        }

        public ScoreContext(bool isInMemory)
        {
            IsInMemory = isInMemory;
        }

        public ScoreContext(DbConnectionStringSupplier connectionStringSupplier) => RunConfiguration(connectionStringSupplier._connectionString);

        public ScoreContext(DbContextOptions<ScoreContext> options, string path) : base(options) => RunConfiguration(path);

        private void RunConfiguration(string path)
        {
            DbPath = Path.Join(path, "Scores.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite( IsInMemory ? "Data Source=InMemorySample;Mode=Memory;Cache=Shared" : $"Data Source={DbPath}");

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ScoreModelDb>().HasKey(m => m.Id);

            builder.Entity<ScoreModelDb>().HasIndex(m => m.Lat);
            builder.Entity<ScoreModelDb>().HasIndex(m => m.Lng);

            base.OnModelCreating(builder);
        }
    }
}