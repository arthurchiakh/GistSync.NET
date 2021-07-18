using System;
using GistSync.Core.Models;

namespace GistSync.Core.Factories.Contracts
{
    public interface IGistWatchFactory
    {
        GistWatch Create(string gistId, GistUpdatedEventHandler gistUpdatedEvent, DateTime? updatedAtUtc = null, string personalAccessToken = null);
    }
}