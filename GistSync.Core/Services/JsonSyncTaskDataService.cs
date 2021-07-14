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
        private readonly ISynchronizedFileAccessService _synchronizedFileAccessService;

        private readonly string _dataFilePath = "./SyncTaskData.json";
        private readonly string _swapTaskDataFilePath = "./SyncTaskData.json.swap";

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
                DefaultJsonDeserializerOptions());
        }

        public void AddOrUpdateTask(SyncTask syncTask)
        {
            var allTasks = GetAllTasks().ToList();

            // If exists then remove existing
            if (allTasks.Any(t => t.Guid == syncTask.Guid))
                allTasks.RemoveAll(t => t.Guid == syncTask.Guid);

            allTasks.Add(syncTask);

            SwapWriteData(allTasks);
        }

        public void RemoveTask(string taskGuid)
        {
            var allTasks = GetAllTasks().ToList();
            allTasks.RemoveAll(t => t.Guid.Equals(taskGuid, StringComparison.OrdinalIgnoreCase));
            SwapWriteData(allTasks);
        }

        private void SwapWriteData(IEnumerable<SyncTask> tasks, CancellationToken ct = default)
        {
            var swapFilePath = _appDataService.GetAbsolutePath(_swapTaskDataFilePath);
            _synchronizedFileAccessService.SynchronizedWriteStream(swapFilePath, FileMode.Create,
                swapFs =>
                {
                    swapFs.Write(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(tasks, DefaultJsonSerializerOptions())));
                    swapFs.Close();


                    // Swap data file
                    var dataFilePath = _appDataService.GetAbsolutePath(_dataFilePath);
                    _fileSystem.File.Move(swapFilePath, dataFilePath, true);
                });
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