using System.Collections.Generic;
using System.Threading.Tasks;
using GistSync.Core.Models;

namespace GistSync.Core.Services.Contracts
{
    public interface ISyncTaskDataService
    {
        IEnumerable<SyncTask> GetAllTasks();
        Task<int> AddOrUpdateTask(SyncTask syncTask);
        Task<int> RemoveTask(int id);
    }
}