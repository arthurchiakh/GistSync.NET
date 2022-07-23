using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GistSync.Core.Models;
using GistSync.Core.Models.GitHub;
using GistSync.Core.Services.Contracts;
using GistSync.Core.Strategies.Contracts;
using Microsoft.Extensions.Logging;

namespace GistSync.Core.Strategies
{
    [RegisterForSyncStrategy(SyncModeTypes.ActiveSync)]
    public class ActiveSyncStrategy : ISyncStrategy
    {
        protected readonly IFileSystem _fileSystem;
        protected readonly IGistWatcherService _gistWatcherService;
        protected readonly IFileChecksumService _fileChecksumService;
        protected readonly ISyncTaskDataService _syncTaskDataService;
        protected readonly ISynchronizedFileAccessService _synchronizedFileAccessService;
        protected readonly IGitHubApiService _gitHubApiService;
        protected readonly INotificationService _notificationService;
        protected readonly ILoggerFactory _loggerFactory;
        private IDisposable _gistWatchUnsubscriber;
        private readonly ILogger _logger;

        internal ActiveSyncStrategy(IFileSystem fileSystem,
                    IGistWatcherService gistWatcherService,
                    IFileChecksumService fileChecksumService, ISyncTaskDataService syncTaskDataService,
                    ISynchronizedFileAccessService synchronizedFileAccessService, IGitHubApiService gitHubApiService,
                    INotificationService notificationService, ILoggerFactory loggerFactory)
        {
            _gistWatcherService = gistWatcherService;
            _fileChecksumService = fileChecksumService;
            _syncTaskDataService = syncTaskDataService;
            _synchronizedFileAccessService = synchronizedFileAccessService;
            _gitHubApiService = gitHubApiService;
            _notificationService = notificationService;
            _loggerFactory = loggerFactory;
            _fileSystem = fileSystem;

            _logger = _loggerFactory.CreateLogger<ActiveSyncStrategy>();
        }

        public ActiveSyncStrategy(IGistWatcherService gistWatcherService,
                                    IFileChecksumService fileChecksumService, ISyncTaskDataService syncTaskDataService,
                                    ISynchronizedFileAccessService synchronizedFileAccessService, IGitHubApiService gitHubApiService,
                                    INotificationService notificationService, ILoggerFactory loggerFactory)
            : this(new FileSystem(), gistWatcherService, fileChecksumService,
                syncTaskDataService, synchronizedFileAccessService, gitHubApiService, notificationService, loggerFactory)
        {
        }

        public virtual void Setup(int syncTaskId)
        {
            _gistWatchUnsubscriber = _gistWatcherService.Subscribe(syncTaskId,
                async gist => { await GistUpdatedHandler(syncTaskId, gist); }
            );
        }

        public virtual void Destroy()
        {
            _gistWatchUnsubscriber.Dispose();
        }

        private async Task GistUpdatedHandler(int syncTaskId, Gist gist)
        {
            var task = await _syncTaskDataService.GetTask(syncTaskId);

            // Skip if the updated date is not changed.
            if (task.UpdatedAt == gist.UpdatedAt) return;

            // Loop through each files and determine need to update the actual or not
            foreach (var file in gist.Files
                         .Values
                         .Where(f => task.Files.Any(tf => tf.FileName.Equals(f.FileName))))
            {
                string newContent;
                if (!file.Truncated.GetValueOrDefault(false))
                    newContent = file.Content;
                else
                    newContent = await _gitHubApiService.GetFileContentByUrl(file.RawUrl);

                var newContentChecksum = await _fileChecksumService.ComputeChecksumByFileContentAsync(newContent);

                var targetFilePath = Path.Combine(task.Directory, file.FileName);

                // If file is not exists then proceed to create one
                if (!_fileSystem.File.Exists(targetFilePath))
                {
                    // Write to the file
                    await _synchronizedFileAccessService.SynchronizedWriteStream(targetFilePath, FileMode.Create,
                        async stream => { await stream.WriteAsync(Encoding.UTF8.GetBytes(newContent)); }
                    );

                    // Update UpdatedAtUtc datetime
                    task.UpdatedAt = gist.UpdatedAt;
                    task.Files.First(f => f.FileName.Equals(file.FileName)).FileChecksum = newContentChecksum;
                    await _syncTaskDataService.UpdateSyncTask(task);

                    // Notification
                    _notificationService.NotifyFileAdded(targetFilePath);
                    _logger.LogInformation($"Sync Task {task.Id}-{task.GistId} - {file.FileName} has been created.");
                }
                else
                {
                    // Compare checksum if file exists
                    var existingContentChecksum = _fileChecksumService.ComputeChecksumByFilePath(targetFilePath);

                    // No need to update if content checksum are the same
                    if (newContentChecksum.Equals(existingContentChecksum, StringComparison.OrdinalIgnoreCase))
                    {
                        task.UpdatedAt = gist.UpdatedAt;
                        await _syncTaskDataService.UpdateSyncTask(task);
                    }
                    else
                    {
                        // Write to the file
                        await _synchronizedFileAccessService.SynchronizedWriteStream(targetFilePath, FileMode.Create,
                            async stream => { await stream.WriteAsync(Encoding.UTF8.GetBytes(newContent)); }
                        );

                        // Update UpdatedAtUtc datetime
                        task.UpdatedAt = gist.UpdatedAt;
                        task.Files.First(f => f.FileName.Equals(file.FileName)).FileChecksum = newContentChecksum;
                        await _syncTaskDataService.UpdateSyncTask(task);

                        // Notification
                        _notificationService.NotifyFileUpdated(targetFilePath);
                        _logger.LogInformation($"Sync Task {task.Id}-{task.GistId} - {file.FileName} has been updated.");
                    }
                }
            }
        }
    }
}