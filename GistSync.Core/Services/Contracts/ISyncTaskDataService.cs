using System.Collections.Generic;
using System.Threading.Tasks;
using GistSync.Core.Models;

namespace GistSync.Core.Services.Contracts
{
    public interface ISyncTaskDataService
    {
        Task<IEnumerable<SyncTask>> GetAllTasks();
        Task<IEnumerable<int>> GetAllEnabledTaskIds();
        Task<SyncTask> GetTask(int id);
        Task<int> AddSyncTask(SyncTask syncTask);
        Task<int> UpdateSyncTask(SyncTask syncTask);
        Task<int> RemoveTask(int id);
        Task<int> EnableTask(int id);
        Task<int> DisableTask(int id);
    }
}