using System;
using GistSync.Core.Models;

namespace GistSync.Core.Services.Contracts
{
    public interface IGistWatcherService
    {
        void AddWatch(GistWatch gistWatch);
        void SetGistUpdatedAtUtc(string gistId, DateTime dateTimeUtc);
    }
}