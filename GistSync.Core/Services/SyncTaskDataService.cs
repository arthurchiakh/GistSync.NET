using System.Collections.Generic;
using GistSync.Core.Models;
using GistSync.Core.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GistSync.Core.Services
{
    public class SyncTaskDataService : ISyncTaskDataService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public SyncTaskDataService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task<int> AddOrUpdateTask(SyncTask syncTask)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<GistSyncDbContext>();

            if (dbContext.SyncTasks.Any(t => t.Id == syncTask.Id))
                dbContext.SyncTasks.Update(syncTask);
            else
                dbContext.SyncTasks.Add(syncTask);

            return dbContext.SaveChangesAsync();
        }

        public Task<IEnumerable<SyncTask>> GetAllTasks()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<GistSyncDbContext>();

            return Task.FromResult<IEnumerable<SyncTask>>(dbContext.SyncTasks.Include(t => t.Files).ToList());
        }

        public async Task<SyncTask> GetTask(int id)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<GistSyncDbContext>();

            return (await dbContext.SyncTasks.FindAsync(id))!;
        }

        public async Task<int> RemoveTask(int id)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<GistSyncDbContext>();

            var syncTask = await dbContext.SyncTasks.FirstOrDefaultAsync(t => t.Id == id);
            if (syncTask == null) return -1;

            dbContext.SyncTasks.Remove(syncTask);
            return await dbContext.SaveChangesAsync();

        }

        public async Task<int> EnableTask(int id)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<GistSyncDbContext>();

            var syncTask = await dbContext.SyncTasks.FirstOrDefaultAsync(t => t.Id == id);
            if (syncTask == null) return -1;

            syncTask.IsEnabled = true;

            dbContext.SyncTasks.Update(syncTask);
            return await dbContext.SaveChangesAsync();
        }

        public async Task<int> DisableTask(int id)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<GistSyncDbContext>();

            var syncTask = await dbContext.SyncTasks.FirstOrDefaultAsync(t => t.Id == id);
            if (syncTask == null) return -1;

            syncTask.IsEnabled = false;

            dbContext.SyncTasks.Update(syncTask);
            return await dbContext.SaveChangesAsync();
        }
    }
}