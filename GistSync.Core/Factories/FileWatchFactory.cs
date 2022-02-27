using System.IO;
using System.IO.Abstractions;
using GistSync.Core.Factories.Contracts;
using GistSync.Core.Models;
using GistSync.Core.Services.Contracts;

namespace GistSync.Core.Factories
{
    public class FileWatchFactory : IFileWatchFactory
    {
        private readonly IFileSystem _fileSystem;
        private readonly IFileChecksumService _fileChecksumService;

        public FileWatchFactory(IFileSystem fileSystem, IFileChecksumService fileChecksumService)
        {
            _fileSystem = fileSystem;
            _fileChecksumService = fileChecksumService;
        }

        public FileWatchFactory(IFileChecksumService fileChecksumService) : this(new FileSystem(), fileChecksumService)
        {
        }

        public FileWatch Create(string filePath, string checksum, FileContentChangedEventHandler fileContentChangedEvent)
        {
            var normalizedFilePath = _fileSystem.Path.GetFullPath(filePath);

            // If file not found, don't write checksum
            var latestChecksum = File.Exists(normalizedFilePath)
                ? _fileChecksumService.ComputeChecksumByFilePath(normalizedFilePath)
                : null;

            var fileWatch = new FileWatch
            {
                FilePath = normalizedFilePath,
                Checksum = (string.IsNullOrEmpty(checksum) ? latestChecksum : checksum) ?? string.Empty,
                ModifiedDateTimeUtc = _fileSystem.File.GetLastWriteTimeUtc(normalizedFilePath)
            };

            fileWatch.FileContentChangedEvent += fileContentChangedEvent;

            // Pick up unhandled file content changed
            if (string.IsNullOrEmpty(checksum) &&
                string.IsNullOrEmpty(latestChecksum) &&
                checksum != latestChecksum)
                fileWatch.TriggerFileContentChanged();

            return fileWatch;
        }
    }
}