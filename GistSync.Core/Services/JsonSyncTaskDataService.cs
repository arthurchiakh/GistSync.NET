using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GistSync.Core.Extensions;
using GistSync.Core.Models;
using GistSync.Core.Services.Contracts;

namespace GistSync.Core.Services
{
    public class JsonSyncTaskDataService : ISyncTaskDataService
    {
        private readonly IFileSystem _fileSystem;
        private readonly IAppDataService _appDataService;

        private readonly string _dataFilePath = "./SyncTaskData.json";
        private readonly string _swapTaskDataFilePath = "./SyncTaskData.json.swap";
        private readonly ReaderWriterLockSlim _readerWriterLock;

        internal JsonSyncTaskDataService(IFileSystem fileSystem, IAppDataService appDataService)
        {
            _fileSystem = fileSystem;
            _appDataService = appDataService;

            _readerWriterLock = new ReaderWriterLockSlim();
        }

        public JsonSyncTaskDataService(IAppDataService appDataService) : this(new FileSystem(), appDataService)
        {
        }

        public SyncTask[] GetAllTasks()
        {
            _readerWriterLock.EnterReadLock();

            try
            {
                return GetAllTasksFromData();
            }
            finally
            {
                if (_readerWriterLock.IsReadLockHeld)
                    _readerWriterLock.ExitReadLock();
            }
        }

        public void AddOrUpdateTask(SyncTask syncTask)
        {
            _readerWriterLock.EnterWriteLock();

            try
            {
                var allTasks = GetAllTasksFromData().ToList();

                // If exists then remove existing
                if (allTasks.Any(t => t.Guid == syncTask.Guid))
                    allTasks.RemoveAll(t => t.Guid == syncTask.Guid);

                allTasks.Add(syncTask);

                SwapData(allTasks);
            }
            finally
            {
                if (_readerWriterLock.IsWriteLockHeld)
                    _readerWriterLock.ExitWriteLock();
            }

        }

        public void RemoveTask(string taskGuid)
        {
            _readerWriterLock.EnterWriteLock();

            try
            {
                var allTasks = GetAllTasksFromData().ToList();
                allTasks.RemoveAll(t => t.Guid.Equals(taskGuid, StringComparison.OrdinalIgnoreCase));

                _readerWriterLock.EnterWriteLock();

                try
                {
                    SwapData(allTasks);
                }
                finally
                {
                    _readerWriterLock.ExitWriteLock(); ;
                }
            }
            finally
            {
                if (_readerWriterLock.IsWriteLockHeld)
                    _readerWriterLock.ExitWriteLock();
            }

        }

        private SyncTask[] GetAllTasksFromData()
        {
            var dataFilePath = _appDataService.GetAbsolutePath(_dataFilePath);

            // If date file not found
            if (!_fileSystem.File.Exists(dataFilePath))
                return Array.Empty<SyncTask>();

            return JsonSerializer.Deserialize<SyncTask[]>(_fileSystem.File.ReadAllTextInReadOnlyMode(dataFilePath),
                                                        DefaultJsonDeserializerOptions());
        }

        private void SwapData(IEnumerable<SyncTask> tasks, CancellationToken ct = default)
        {
            // Create swap data file
            var swapFilePath = _appDataService.GetAbsolutePath(_swapTaskDataFilePath);
            using var swapFs = _fileSystem.File.Open(swapFilePath, FileMode.Create);
            swapFs.Write(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(tasks, DefaultJsonSerializerOptions())));
            swapFs.Dispose();

            // Swap data file
            var dataFilePath = _appDataService.GetAbsolutePath(_dataFilePath);
            _fileSystem.File.Move(swapFilePath, dataFilePath, true);
        }

        private JsonSerializerOptions DefaultJsonDeserializerOptions() => new()
        {
            PropertyNameCaseInsensitive = true,
        };

        private JsonSerializerOptions DefaultJsonSerializerOptions() => new()
        {
            WriteIndented = false
        };
    }
}