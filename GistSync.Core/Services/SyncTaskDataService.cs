using System.Collections.Generic;
using GistSync.Core.Models;
using GistSync.Core.Services.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GistSync.Core.Services
{
    public class SyncTaskDataService : ISyncTaskDataService
    {
        private readonly GistSyncDbContext _dbContext;

        public SyncTaskDataService(GistSyncDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<int> AddOrUpdateTask(SyncTask syncTask)
        {
            if (_dbContext.SyncTasks.Any(t => t.GistId == syncTask.GistId))
                _dbContext.SyncTasks.Update(syncTask);
            else
                _dbContext.SyncTasks.Add(syncTask);

            return _dbContext.SaveChangesAsync();
        }

        public IEnumerable<SyncTask> GetAllTasks()
        {
            return _dbContext.SyncTasks.Include(t => t.Files).ToList();
        }

        public async Task<int> RemoveTask(int id)
        {
            var syncTask = await _dbContext.SyncTasks.FirstOrDefaultAsync(t => t.Id == id);

            if (syncTask != null)
            {
                _dbContext.SyncTasks.Remove(syncTask);
                return await _dbContext.SaveChangesAsync();
            }

            return 0;
        }
    }
}