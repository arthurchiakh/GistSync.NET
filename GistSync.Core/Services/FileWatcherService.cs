using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using GistSync.Core.Services.Contracts;

namespace GistSync.Core.Services
{
    public class FileWatcherService : IFileWatcherService
    {
        private readonly IFileSystem _fileSystem;
        private readonly IFileChecksumService _fileChecksumService;
        private readonly ISyncTaskDataService _syncTaskDataService;
        private readonly ConcurrentDictionary<int, ICollection<IFileSystemWatcher>> _items;
        private readonly HashSet<string> _updatingFilePath;

        internal FileWatcherService(IFileSystem fileSystem, IFileChecksumService fileChecksumService,
                                    ISyncTaskDataService syncTaskDataService)
        {
            _fileSystem = fileSystem;
            _fileChecksumService = fileChecksumService;
            _syncTaskDataService = syncTaskDataService;
            _items = new ConcurrentDictionary<int, ICollection<IFileSystemWatcher>>();
            _updatingFilePath = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        public FileWatcherService(IFileChecksumService fileChecksumService, ISyncTaskDataService syncTaskDataService)
            : this(new FileSystem(), fileChecksumService, syncTaskDataService)
        {
        }

        ~FileWatcherService()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (!_items.Any()) return;

            foreach (var syncTaskId in _items.Keys)
            {
                if (!_items.TryRemove(syncTaskId, out var fileSystemWatcherList) ||
                    !fileSystemWatcherList.Any()) return;

                foreach (var watcher in fileSystemWatcherList) watcher.Dispose();
            }
        }

        public async Task<IDisposable> Subscribe(int syncTaskId, Action<string, string> fileModifiedHandler)
        {
            var syncTask = await _syncTaskDataService.GetTask(syncTaskId);

            if (_items.ContainsKey(syncTaskId)) return new Unsubscriber(_items, syncTaskId);

            var fileSystemWatcherList = new List<IFileSystemWatcher>();

            foreach (var file in syncTask.Files)
            {
                var watcher = _fileSystem.FileSystemWatcher.CreateNew(syncTask.Directory);
                watcher.NotifyFilter = NotifyFilters.LastWrite;
                watcher.Filter = file.FileName;
                watcher.IncludeSubdirectories = false;
                watcher.EnableRaisingEvents = true;

                watcher.Changed += (_, args) =>
                {
                    // Skip if any other thread is updating
                    // To prevent FileSystemWatcher to trigger OnChanged for multiple times.
                    if (_updatingFilePath.Contains(args.FullPath)) return;
                    _updatingFilePath.Add(args.FullPath);

                    Task.Run(() =>
                    {
                        try
                        {
                            fileModifiedHandler(args.FullPath,
                                _fileChecksumService.ComputeChecksumByFilePath(args.FullPath));
                        }
                        finally
                        {
                            _updatingFilePath.Remove(args.FullPath);
                        }
                    });
                };

                fileSystemWatcherList.Add(watcher);
            }

            _items.TryAdd(syncTaskId, fileSystemWatcherList);

            return new Unsubscriber(_items, syncTaskId);
        }

        private class Unsubscriber : IDisposable
        {
            private readonly ConcurrentDictionary<int, ICollection<IFileSystemWatcher>> _items;
            private readonly int _syncTaskId;

            public Unsubscriber(ConcurrentDictionary<int, ICollection<IFileSystemWatcher>> items, int syncTaskId)
            {
                _items = items;
                _syncTaskId = syncTaskId;
            }

            public void Dispose()
            {
                if (!_items.TryRemove(_syncTaskId, out var fileSystemWatcherList) ||
                    !fileSystemWatcherList.Any()) return;

                foreach (var watcher in fileSystemWatcherList) watcher.Dispose();
            }
        }
    }
}
