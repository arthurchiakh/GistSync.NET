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

        public FileWatch Create(string filePath)
        {
            var standardizedFilePath = _fileSystem.Path.GetFullPath(filePath);

            return new FileWatch
            {
                FilePath = standardizedFilePath,
                FileHash = _fileChecksumService.ComputeChecksumByFilePath(standardizedFilePath),
                ModifiedDateTimeUtc = _fileSystem.File.GetLastWriteTimeUtc(standardizedFilePath)
            };
        }
    }
}