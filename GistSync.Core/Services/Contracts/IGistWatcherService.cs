using GistSync.Core.Models;

namespace GistSync.Core.Services.Contracts
{
    public interface IGistWatcherService
    {
        void AddWatch(GistWatch gistWatch);
    }
}