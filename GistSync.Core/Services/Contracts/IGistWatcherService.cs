using System;
using GistSync.Core.Models;

namespace GistSync.Core.Services.Contracts
{
    public interface IGistWatcherService
    {
        IDisposable Subscribe(GistWatch gistWatch);

        /// <summary>
        /// Override Gist updated date to prevent unnecessary sync action after the gist updated by own.
        /// </summary>
        /// <param name="syncTaskId">Sync Task Id</param>
        /// <param name="dateTimeUtc">UTC DateTime</param>
        void OverrideGistUpdatedAtUtc(int syncTaskId, DateTime dateTimeUtc);

        void ReloadSettings();
    }
}