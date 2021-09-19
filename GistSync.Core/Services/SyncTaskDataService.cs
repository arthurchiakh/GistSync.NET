using GistSync.Core.Models;
using GistSync.Core.Services.Contracts;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace GistSync.Core.Services
{
    public class SyncTaskDataService : ISyncTaskDataService
    {
        private readonly IServiceScopeFactory scopeFactory;

        public SyncTaskDataService(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        public void AddOrUpdateTask(SyncTask syncTask)
        {
            using var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GistSyncDbContext>();
            context.SyncTasks.Update(syncTask);
        }

        public SyncTask[] GetAllTasks()
        {
            using var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GistSyncDbContext>();
            return context.SyncTasks.ToArray();
        }

        public void RemoveTask(string gistId)
        {
            using var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GistSyncDbContext>();
            context.SyncTasks.Remove(new SyncTask { GistId = gistId });
        }
    }
}