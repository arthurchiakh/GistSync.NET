using System.Threading;
using System.Threading.Tasks;
using GistSync.Core.Models;

namespace GistSync.Core.Services.Contracts
{
    public interface ISyncTaskDataService
    {
        Task<SyncTask[]> GetAllTasks(CancellationToken ct = default);
        Task AddTask(SyncTask syncTask, CancellationToken ct = default);
        Task RemoveTask(string taskGuid, CancellationToken ct = default);
    }
}