using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
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

        public async Task<SyncTask[]> GetAllTasks(CancellationToken ct = default(CancellationToken))
        {
            try
            {
                if (!_readerWriterLock.TryEnterReadLock(TimeSpan.FromMinutes(5)))
                    throw new TimeoutException("ReadWriteLock timed out when reading getting all sync tasks.");

                var dataFilePath = _appDataService.GetAbsolutePath(_dataFilePath);

                // If date file not found
                if (!_fileSystem.File.Exists(dataFilePath))
                    return Array.Empty<SyncTask>();

                await using var fileStream = _fileSystem.File.OpenRead(dataFilePath);
                var allTasks = await JsonSerializer.DeserializeAsync<SyncTask[]>(fileStream, DefaultJsonDeserializerOptions(), ct);

                return allTasks;
            }
            finally
            {
                if (_readerWriterLock.IsReadLockHeld)
                    _readerWriterLock.ExitReadLock();
            }
        }

        public async Task AddOrUpdateTask(SyncTask syncTask, CancellationToken ct = default(CancellationToken))
        {
            try
            {
                if (!_readerWriterLock.TryEnterUpgradeableReadLock(TimeSpan.FromMinutes(5)))
                    throw new TimeoutException("ReadWriteLock timed out when reading getting all sync tasks.");

                var allTasks = (await GetAllTasks(ct)).ToList();

                // If exists then remove existing
                if (allTasks.Any(t => t.Guid == syncTask.Guid))
                    allTasks.RemoveAll(t => t.Guid == syncTask.Guid);

                allTasks.Add(syncTask);

                await SwapData(allTasks, ct);
            }
            finally
            {
                if (_readerWriterLock.IsUpgradeableReadLockHeld)
                    _readerWriterLock.ExitUpgradeableReadLock();
            }
        }

        public async Task RemoveTask(string taskGuid, CancellationToken ct = default)
        {
            try
            {
                if (!_readerWriterLock.TryEnterUpgradeableReadLock(TimeSpan.FromMinutes(5)))
                    throw new TimeoutException("ReadWriteLock timed out when reading getting all sync tasks.");

                var allTasks = (await GetAllTasks(ct)).ToList();
                allTasks.RemoveAll(t => t.Guid.Equals(taskGuid, StringComparison.OrdinalIgnoreCase));

                await SwapData(allTasks, ct);
            }
            finally
            {
                if (_readerWriterLock.IsUpgradeableReadLockHeld)
                    _readerWriterLock.ExitUpgradeableReadLock();
            }
        }

        private async Task SwapData(IEnumerable<SyncTask> tasks, CancellationToken ct = default(CancellationToken))
        {
            // Create swap data file
            var swapFilePath = _appDataService.GetAbsolutePath(_swapTaskDataFilePath);
            await using var swapFs = _fileSystem.File.Open(swapFilePath, FileMode.Create);
            await JsonSerializer.SerializeAsync(swapFs, tasks, DefaultJsonSerializerOptions(), ct);
            await swapFs.DisposeAsync();

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