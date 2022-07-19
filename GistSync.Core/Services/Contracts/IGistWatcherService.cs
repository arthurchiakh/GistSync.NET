using System;
using GistSync.Core.Models.GitHub;

namespace GistSync.Core.Services.Contracts
{
    public interface IGistWatcherService : IDisposable
    {
        /// <summary>
        /// Subscribe to GistWatcherService with SyncTaskId and GistUpdatedHandler provides updated Gist object.
        /// </summary>
        /// <param name="syncTaskId"></param>
        /// <param name="gistUpdatedHandler"></param>
        /// <returns></returns>
        IDisposable Subscribe(int syncTaskId, Action<Gist> gistUpdatedHandler);
        /// <summary>
        /// Reload configuration settings
        /// </summary>
        void ReloadConfigurationSettings();
    }
}