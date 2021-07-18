using GistSync.Core.Models;

namespace GistSync.Core.Services.Contracts
{
    public interface ISyncTaskDataService
    {
        SyncTask[] GetAllTasks();
        void AddOrUpdateTask(SyncTask syncTask);
        void RemoveTask(string gistId);
    }
}