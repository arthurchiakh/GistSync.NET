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
            var standardizedFilePath = StandardizeFilePath(filePath);

            return new FileWatch
            {
                FilePath = standardizedFilePath,
                FileHash = _fileChecksumService.ComputeFileChecksum(standardizedFilePath),
                ModifiedDateTimeUtc = _fileSystem.File.GetLastWriteTimeUtc(standardizedFilePath)
            };
        }

        /// <summary>
        ///  Standardize slash and back slash characterV
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <returns>Standardized file path</returns>
        private string StandardizeFilePath(string filePath)
        {
            return _fileSystem.Path.GetFullPath(filePath);
        }
    }
}
