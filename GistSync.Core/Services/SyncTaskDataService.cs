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

        public async Task<int> AddSyncTask(SyncTask syncTask)
        {
            await using var scope = _serviceScopeFactory.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<GistSyncDbContext>();

            dbContext.SyncTasks.Add(syncTask);

            return await dbContext.SaveChangesAsync();
        }

        public async Task<int> UpdateSyncTask(SyncTask syncTask)
        {
            await using var scope = _serviceScopeFactory.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<GistSyncDbContext>();
            dbContext.SyncTasks.Update(syncTask);

            return await dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<SyncTask>> GetAllTasks()
        {
            await using var scope = _serviceScopeFactory.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<GistSyncDbContext>();

            return await dbContext.SyncTasks.Include(t => t.Files).AsNoTracking().ToArrayAsync();
        }

        public async Task<IEnumerable<int>> GetAllEnabledTaskIds()
        {
            await using var scope = _serviceScopeFactory.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<GistSyncDbContext>();

            return await dbContext.SyncTasks.Where(t => t.IsEnabled).Select(t => t.Id).ToArrayAsync();
        }

        public async Task<SyncTask> GetTask(int id)
        {
            await using var scope = _serviceScopeFactory.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<GistSyncDbContext>();

            return await dbContext.SyncTasks.Include(t => t.Files)
                .SingleAsync(t => t.Id == id);
        }

        public async Task<int> RemoveTask(int id)
        {
            await using var scope = _serviceScopeFactory.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<GistSyncDbContext>();

            var syncTask = await dbContext.SyncTasks.FirstOrDefaultAsync(t => t.Id == id);
            if (syncTask == null) return -1;

            dbContext.SyncTasks.Remove(syncTask);
            return await dbContext.SaveChangesAsync();

        }

        public async Task<int> EnableTask(int id)
        {
            await using var scope = _serviceScopeFactory.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<GistSyncDbContext>();

            var syncTask = await dbContext.SyncTasks.FirstOrDefaultAsync(t => t.Id == id);
            if (syncTask == null) return -1;

            syncTask.IsEnabled = true;

            dbContext.SyncTasks.Update(syncTask);
            return await dbContext.SaveChangesAsync();
        }

        public async Task<int> DisableTask(int id)
        {
            await using var scope = _serviceScopeFactory.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<GistSyncDbContext>();

            var syncTask = await dbContext.SyncTasks.FirstOrDefaultAsync(t => t.Id == id);
            if (syncTask == null) return -1;

            syncTask.IsEnabled = false;

            dbContext.SyncTasks.Update(syncTask);
            return await dbContext.SaveChangesAsync();
        }
    }
}