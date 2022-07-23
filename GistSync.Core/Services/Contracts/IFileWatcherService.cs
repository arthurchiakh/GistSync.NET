using System;
using System.Threading.Tasks;

namespace GistSync.Core.Services.Contracts
{
    public interface IFileWatcherService : IDisposable
    {
        /// <summary>
        ///  Subscribe to file watcher service with SyncTaskId and File Modified Handler provides modified file path and latest checksum
        /// </summary>
        /// <param name="syncTaskId">Sync Task Id</param>
        /// <param name="fileModifiedHandler">Handler provides modified file path and latest checksum</param>
        /// <returns>Disposable Typed Unsubscriber</returns>
        Task<IDisposable> Subscribe(int syncTaskId, Action<string, string> fileModifiedHandler);
    }
}