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
            var latestChecksum = _fileChecksumService.ComputeChecksumByFilePath(normalizedFilePath);

            var fileWatch = new FileWatch
            {
                FilePath = normalizedFilePath,
                Checksum = checksum ?? latestChecksum,
                ModifiedDateTimeUtc = _fileSystem.File.GetLastWriteTimeUtc(normalizedFilePath)
            };

            fileWatch.FileContentChangedEvent += fileContentChangedEvent;

            // Pick up unhandled file content changed
            if (checksum != null && checksum != latestChecksum)
               fileWatch.TriggerFileContentChanged();

            return fileWatch;
        }
    }
}