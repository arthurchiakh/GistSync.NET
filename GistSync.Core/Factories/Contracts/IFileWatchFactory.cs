using GistSync.Core.Models;

namespace GistSync.Core.Factories.Contracts
{
    public interface IFileWatchFactory
    {
        public FileWatch Create(string filePath, string checksum, FileContentChangedEventHandler fileContentChangedEvent);
    }
}