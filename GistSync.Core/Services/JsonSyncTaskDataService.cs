using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Text.Json;
using GistSync.Core.Models;
using GistSync.Core.Services.Contracts;

namespace GistSync.Core.Services
{
    public class JsonSyncTaskDataService : ISyncTaskDataService
    {
        private readonly IFileSystem _fileSystem;
        private readonly IAppDataService _appDataService;
        private readonly ISynchronizedFileAccessService _synchronizedFileAccessService;

        private readonly string _dataFilePath = "./sync-tasks.json";
        private readonly string _swapTaskDataFilePath = "./sync-tasks.json.swap";

        internal JsonSyncTaskDataService(IFileSystem fileSystem, IAppDataService appDataService,
                                        ISynchronizedFileAccessService synchronizedFileAccessService)
        {
            _fileSystem = fileSystem;
            _appDataService = appDataService;
            _synchronizedFileAccessService = synchronizedFileAccessService;
        }

        public JsonSyncTaskDataService(IAppDataService appDataService, ISynchronizedFileAccessService synchronizedFileAccessService)
                                        : this(new FileSystem(), appDataService, synchronizedFileAccessService)
        {
        }

        public SyncTask[] GetAllTasks()
        {
            var dataFilePath = _appDataService.GetAbsolutePath(_dataFilePath);

            // If date file not found
            if (!_fileSystem.File.Exists(dataFilePath))
                return Array.Empty<SyncTask>();

            return JsonSerializer.Deserialize<SyncTask[]>(_synchronizedFileAccessService.SynchronizedReadAllText(dataFilePath),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public void AddOrUpdateTask(SyncTask syncTask)
        {
            var allTasks = GetAllTasks().ToList();

            // If exists then remove existing
            if (allTasks.Any(t => t.GistId.Equals(syncTask.GistId, StringComparison.OrdinalIgnoreCase)))
                allTasks.RemoveAll(t => t.GistId.Equals(syncTask.GistId, StringComparison.OrdinalIgnoreCase));

            allTasks.Add(syncTask);

            SwapWriteData(allTasks);
        }

        public void RemoveTask(string gistId)
        {
            var allTasks = GetAllTasks().ToList();
            allTasks.RemoveAll(t => t.GistId.Equals(gistId, StringComparison.OrdinalIgnoreCase));
            SwapWriteData(allTasks);
        }

        private void SwapWriteData(IEnumerable<SyncTask> tasks)
        {
            var swapFilePath = _appDataService.GetAbsolutePath(_swapTaskDataFilePath);
            _synchronizedFileAccessService.SynchronizedWriteStream(swapFilePath, FileMode.Create,
                swapFs =>
                {
                    swapFs.Write(Encoding.UTF8.GetBytes(
                        JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = false })
                    ));
                    swapFs.Close();

                    // Swap data file
                    var dataFilePath = _appDataService.GetAbsolutePath(_dataFilePath);
                    _fileSystem.File.Move(swapFilePath, dataFilePath, true);
                });
        }
    }
}