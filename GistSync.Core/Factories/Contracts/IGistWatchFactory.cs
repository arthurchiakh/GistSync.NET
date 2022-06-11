using System;
using GistSync.Core.Models;

namespace GistSync.Core.Factories.Contracts
{
    public interface IGistWatchFactory
    {
        GistWatch Create(SyncTask syncTask, GistUpdatedEventHandler gistUpdatedEvent, DateTime? updatedAtUtc = null, string personalAccessToken = null);
    }
}