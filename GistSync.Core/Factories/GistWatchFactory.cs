using GistSync.Core.Models;

namespace GistSync.Core.Factories
{
    public class GistWatchFactory
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