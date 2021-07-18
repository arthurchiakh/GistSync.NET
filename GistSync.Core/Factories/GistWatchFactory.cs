using System;
using GistSync.Core.Factories.Contracts;
using GistSync.Core.Models;

namespace GistSync.Core.Factories
{
    public class GistWatchFactory : IGistWatchFactory
    {
        public GistWatch Create(string gistId, GistUpdatedEventHandler gistUpdatedEvent, DateTime? updatedAtUtc = null, string personalAccessToken = null)
        {
            var gistWatch = new GistWatch
            {
                GistId = gistId,
                UpdatedAtUtc = updatedAtUtc,
                PersonalAccessToken = personalAccessToken,
            };

            gistWatch.GistUpdatedEvent += gistUpdatedEvent;

            return gistWatch;
        }
    }
}