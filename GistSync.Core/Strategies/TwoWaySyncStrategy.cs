using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using GistSync.Core.Models;
using GistSync.Core.Models.GitHub;
using GistSync.Core.Services.Contracts;
using Microsoft.Extensions.Logging;

namespace GistSync.Core.Strategies
{
    [RegisterForSyncStrategy(SyncModeTypes.TwoWaySync)]
    public class TwoWaySyncStrategy : ActiveSyncStrategy
    {
        private readonly IFileWatcherService _fileWatcherService;
        private IDisposable _fileWatchUnsubscriber;
        private readonly ILogger _logger;

        internal TwoWaySyncStrategy(IFileSystem fileSystem, IFileWatcherService fileWatcherService,
            IGitHubApiService gitHubApiService,
            ISynchronizedFileAccessService synchronizedFileAccessService, IGistWatcherService gistWatcherService,
            IFileChecksumService fileChecksumService, ISyncTaskDataService syncTaskDataService,
            INotificationService notificationService,
            ILoggerFactory loggerFactory) :
            base(fileSystem, gistWatcherService, fileChecksumService,
                syncTaskDataService, synchronizedFileAccessService, gitHubApiService, notificationService,
                loggerFactory)
        {
            _fileWatcherService = fileWatcherService;

            _logger = _loggerFactory.CreateLogger<TwoWaySyncStrategy>();
        }

        public TwoWaySyncStrategy(IFileWatcherService fileWatcherService,
            IGitHubApiService gitHubApiService,
            ISynchronizedFileAccessService synchronizedFileAccessService, IGistWatcherService gistWatcherService,
            IFileChecksumService fileChecksumService, ISyncTaskDataService syncTaskDataService,
            INotificationService notificationService, ILoggerFactory loggerFactory)
            : this(new FileSystem(), fileWatcherService,
                gitHubApiService, synchronizedFileAccessService, gistWatcherService, fileChecksumService,
                syncTaskDataService, notificationService, loggerFactory)
        {
        }

        public override void Setup(int syncTaskId)
        {
            // Setup for ActiveSync
            base.Setup(syncTaskId);

            // Setup file watch
            _fileWatchUnsubscriber = _fileWatcherService
                .Subscribe(syncTaskId, async (fullPath, checksum) =>
            {
                await FileUpdatedHandler(syncTaskId, fullPath, checksum);
            });
        }

        public override void Destroy()
        {
            base.Destroy();
            _fileWatchUnsubscriber.Dispose();
        }

        private async Task FileUpdatedHandler(int syncTaskId, string fileFullPath, string newChecksum)
        {
            var updatedFileName = _fileSystem.Path.GetFileName(fileFullPath);
            var task = await _syncTaskDataService.GetTask(syncTaskId);

            var file = task.Files.FirstOrDefault(f => f.FileName == updatedFileName);

            if (file == null) return;

            if (file.FileChecksum == newChecksum) return;

            var content = _synchronizedFileAccessService.ReadAllText(fileFullPath);

            var updatedGist = await _gitHubApiService.PatchGist(task.GistId, new GistPatch
            {
                Files = new Dictionary<string, FilePatch>
                {
                    {
                        file.FileName, new FilePatch
                        {
                            FileName = file.FileName,
                            Content = content
                        }
                    }
                }
            }, task.GitHubPersonalAccessToken!);

            // Update UpdatedAtUtc datetime
            task.UpdatedAt = updatedGist.UpdatedAt;
            file.FileChecksum = newChecksum;
            await _syncTaskDataService.UpdateSyncTask(task);

            // Notification
            _notificationService.NotifyGistUpdated(task.GistId);
            _logger.LogInformation($"Sync Task {task.Id}-{task.GistId} - Gist {task.GistId} has been updated.");
        }
    }
}