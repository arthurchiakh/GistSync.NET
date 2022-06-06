using System.Collections.Generic;
using System.Threading.Tasks;
using GistSync.Core.Models;

namespace GistSync.Core.Services.Contracts
{
    public interface ISyncTaskDataService
    {
        Task<IEnumerable<SyncTask>> GetAllTasks();
        Task<SyncTask> GetTask(int id);
        Task<int> AddOrUpdateTask(SyncTask syncTask);
        Task<int> RemoveTask(int id);
        Task<int> EnableTask(int id);
        Task<int> DisableTask(int id);
    }
}