using System;
using System.Diagnostics.CodeAnalysis;
using GistSync.Core.Models;

namespace GistSync.Core.Services.Contracts
{
    public interface IFileWatcherService
    {
        IDisposable Subscribe([NotNull] FileWatch fileWatch);
    }
}