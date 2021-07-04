using GistSync.Core.Factories.Contracts;
using GistSync.Core.Models;

namespace GistSync.Core.Factories
{
    public class GistWatchFactory : IGistWatchFactory
    {
        public GistWatch Create(string gistId, string personalAccessToken = null)
        {
            return new GistWatch
            {
                GistId = gistId,
                PersonalAccessToken = personalAccessToken
            };
        }
    }
}