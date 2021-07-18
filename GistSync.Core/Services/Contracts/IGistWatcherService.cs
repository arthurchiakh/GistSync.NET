using System;
using GistSync.Core.Models;

namespace GistSync.Core.Services.Contracts
{
    public interface IGistWatcherService
    {
        IDisposable Subscribe(GistWatch gistWatch);
        void OverrideGistUpdatedAtUtc(string gistId, DateTime dateTimeUtc);
    }
}