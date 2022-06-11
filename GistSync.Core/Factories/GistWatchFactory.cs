using System;
using GistSync.Core.Factories.Contracts;
using GistSync.Core.Models;

namespace GistSync.Core.Factories
{
    public class GistWatchFactory : IGistWatchFactory
    {
        public GistWatch Create(SyncTask syncTask, GistUpdatedEventHandler gistUpdatedEvent, DateTime? updatedAtUtc = null, string personalAccessToken = null)
        {
            var gistWatch = new GistWatch
            {
                SyncTaskId = syncTask.Id,
                GistId = syncTask.GistId,
                UpdatedAtUtc = updatedAtUtc,
                PersonalAccessToken = personalAccessToken,
            };

            gistWatch.GistUpdatedEvent += gistUpdatedEvent;

            return gistWatch;
        }
    }
}