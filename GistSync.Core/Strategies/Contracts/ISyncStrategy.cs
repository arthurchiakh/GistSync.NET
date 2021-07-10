using GistSync.Core.Models;

namespace GistSync.Core.Strategies.Contracts
{
    public interface ISyncStrategy
    {
        void Setup(SyncTask task);
    }
}