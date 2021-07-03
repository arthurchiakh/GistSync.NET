using System.Diagnostics.CodeAnalysis;
using GistSync.Core.Models;

namespace GistSync.Core.Services.Contracts
{
    public interface IFileWatcherService
    {
        void AddWatch([NotNull] FileWatch fileWatch);
    }
}