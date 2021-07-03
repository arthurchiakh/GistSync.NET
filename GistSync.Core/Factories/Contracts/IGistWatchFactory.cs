using GistSync.Core.Models;

namespace GistSync.Core.Factories.Contracts
{
    public interface IGistWatchFactory
    {
        GistWatch Create(string gistId, string personalAccessToken = null);
    }
}