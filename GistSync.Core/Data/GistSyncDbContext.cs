using System.Diagnostics;
using GistSync.Core.Models;
using GistSync.Core.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace GistSync.Core
{
    public sealed class GistSyncDbContext : DbContext
    {
        private readonly IAppDataService _appDataService;
        public DbSet<SyncTask> SyncTasks { get; set; }
        public DbSet<Setting> Settings { get; set; }

        public string DbPath { get; }

        public GistSyncDbContext(IAppDataService appDataService, DbContextOptions<GistSyncDbContext> options)
            : base(options)
        {
            _appDataService = appDataService;
            DbPath = _appDataService.GetAbsolutePath("GistSync.NET.db");
            Database.EnsureCreated();
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={DbPath}");
        }
    }
}