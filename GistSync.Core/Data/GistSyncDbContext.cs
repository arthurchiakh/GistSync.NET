using System;
using System.IO;
using GistSync.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace GistSync.Core
{
    public class GistSyncDbContext : DbContext
    {
        public DbSet<SyncTask> SyncTasks { get; set; }

        public string DbPath { get; }

        public GistSyncDbContext(DbContextOptions<GistSyncDbContext> options)
            : base(options)
        {
            DbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "GistSync.NET",
                "GistSync.NET.db");
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SyncTask>()
                .ToTable("SyncTasks")
                .HasKey(t => t.GistId);
        }
    }
}