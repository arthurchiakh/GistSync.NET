using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using GistSync.Core.Models;
using GistSync.Core.Services.Contracts;

namespace GistSync.Core.Services
{
    public class FileWatcherService : IFileWatcherService
    {
        private readonly IFileSystem _fileSystem;
        private readonly IFileChecksumService _fileChecksumService;
        private readonly IDictionary<string, IFileSystemWatcher> _fileSystemWatchers;
        private readonly IDictionary<string, IList<FileWatch>> _fileWatches;

        internal FileWatcherService(IFileSystem fileSystem, IFileChecksumService fileChecksumService)
        {
            _fileSystem = fileSystem;
            _fileChecksumService = fileChecksumService;
            _fileSystemWatchers = new Dictionary<string, IFileSystemWatcher>();
            _fileWatches = new Dictionary<string, IList<FileWatch>>();
        }

        public FileWatcherService(IFileChecksumService fileChecksumService) : this(new FileSystem(), fileChecksumService)
        {
        }

        ~FileWatcherService()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_fileSystemWatchers != null && _fileSystemWatchers.Count > 0)
                foreach (var watcher in _fileSystemWatchers.Values) watcher?.Dispose();
        }

        public IDisposable Subscribe([NotNull] FileWatch fileWatch)
        {
            if (fileWatch == null)
                throw new ArgumentNullException(nameof(fileWatch)); // GIGO

            if (string.IsNullOrWhiteSpace(fileWatch.FilePath))
                throw new ArgumentNullException(fileWatch.FilePath); // GIGO

            if (!_fileSystem.File.Exists(fileWatch.FilePath))
                throw new FileNotFoundException(fileWatch.FilePath);

            // Register file watch
            if (!_fileWatches.ContainsKey(fileWatch.FilePath))
                _fileWatches[fileWatch.FilePath] = new List<FileWatch> { fileWatch };
            else
                _fileWatches[fileWatch.FilePath].Add(fileWatch);

            // Register file system watcher
            if (!_fileSystemWatchers.ContainsKey(fileWatch.FilePath))
            {
                var watcher = _fileSystem.FileSystemWatcher.CreateNew(Path.GetDirectoryName(fileWatch.FilePath));
                watcher.NotifyFilter = NotifyFilters.LastWrite;
                watcher.Filter = Path.GetFileName(fileWatch.FilePath);
                watcher.IncludeSubdirectories = false;
                watcher.EnableRaisingEvents = true;

                _fileSystemWatchers[fileWatch.FilePath] = watcher;
                _fileSystemWatchers[fileWatch.FilePath].Changed += OnFileChanged;
            }

            return new Unsubscriber(_fileSystemWatchers, _fileWatches, fileWatch);
        }

        private void OnFileChanged(object sender, FileSystemEventArgs args)
        {
            var normalizedFilePath = _fileSystem.Path.GetFullPath(args.FullPath);
            // Compute file hash for comparison
            var newFileHash = _fileChecksumService.ComputeChecksumByFilePath(normalizedFilePath);
            var modifiedDate = File.GetLastWriteTimeUtc(normalizedFilePath);
            foreach (var watch in _fileWatches[normalizedFilePath])
            {
                if (!watch.Checksum.Equals(newFileHash, StringComparison.OrdinalIgnoreCase))
                {
                    watch.ModifiedDateTimeUtc = modifiedDate;
                    watch.Checksum = newFileHash;
                    watch.TriggerFileContentChanged();
                }
            }
        }

        private class Unsubscriber : IDisposable
        {
            private readonly IDictionary<string, IFileSystemWatcher> _fileSystemWatchers;
            private readonly IDictionary<string, IList<FileWatch>> _fileWatches;
            private readonly FileWatch _fileWatch;

            public Unsubscriber(IDictionary<string, IFileSystemWatcher> fileSystemWatchers, IDictionary<string, IList<FileWatch>> fileWatches,
                FileWatch fileWatch)
            {
                _fileSystemWatchers = fileSystemWatchers;
                _fileWatch = fileWatch;
                _fileWatches = fileWatches;
            }

            public void Dispose()
            {
                if (_fileSystemWatchers.TryGetValue(_fileWatch.FilePath, out var fileSystemWatcher))
                    fileSystemWatcher.Dispose();

                _fileSystemWatchers.Remove(_fileWatch.FilePath);
                _fileWatches.Remove(_fileWatch.FilePath);
            }
        }
    }
}
