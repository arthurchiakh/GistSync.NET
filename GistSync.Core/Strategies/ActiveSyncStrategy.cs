using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using GistSync.Core.Factories.Contracts;
using GistSync.Core.Models;
using GistSync.Core.Services.Contracts;
using GistSync.Core.Strategies.Contracts;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;

namespace GistSync.Core.Strategies
{
    [RegisterForSyncStrategy(SyncModeTypes.ActiveSync)]
    public class ActiveSyncStrategy : ISyncStrategy
    {
        protected SyncTask _syncTask;
        protected readonly IFileSystem _fileSystem;
        protected readonly IGistWatchFactory _gistWatchFactory;
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
                    IGistWatchFactory gistWatchFactory, IGistWatcherService gistWatcherService,
                    IFileChecksumService fileChecksumService, ISyncTaskDataService syncTaskDataService,
                    ISynchronizedFileAccessService synchronizedFileAccessService, IGitHubApiService gitHubApiService,
                    INotificationService notificationService, ILoggerFactory loggerFactory)
        {
            _gistWatchFactory = gistWatchFactory;
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

        public ActiveSyncStrategy(IGistWatchFactory gistWatchFactory, IGistWatcherService gistWatcherService,
                                    IFileChecksumService fileChecksumService, ISyncTaskDataService syncTaskDataService,
                                    ISynchronizedFileAccessService synchronizedFileAccessService, IGitHubApiService gitHubApiService,
                                    INotificationService notificationService, ILoggerFactory loggerFactory)
            : this(new FileSystem(), gistWatchFactory, gistWatcherService, fileChecksumService,
                syncTaskDataService, synchronizedFileAccessService, gitHubApiService, notificationService, loggerFactory)
        {
        }

        public virtual void Setup(SyncTask task)
        {
            _syncTask = task;
            // Create watch object
            var gistWatch = _gistWatchFactory.Create(task,
                async (sender, args) =>
                {
                    foreach (var file in args.Files.Where(f => task.Files.Any(tf => tf.FileName.Equals(f.FileName))))
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
                            task.UpdatedAt = args.UpdatedAtUtc;
                            task.Files.First(f => f.FileName.Equals(file.FileName)).FileChecksum = newContentChecksum;
                            await _syncTaskDataService.AddOrUpdateTask(task);

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
                                task.UpdatedAt = args.UpdatedAtUtc;
                                await _syncTaskDataService.AddOrUpdateTask(task);
                            }
                            else
                            {
                                // Write to the file
                                await _synchronizedFileAccessService.SynchronizedWriteStream(targetFilePath, FileMode.Create,
                                    async stream => { await stream.WriteAsync(Encoding.UTF8.GetBytes(newContent)); }
                                );

                                // Update UpdatedAtUtc datetime
                                task.UpdatedAt = args.UpdatedAtUtc;
                                task.Files.First(f => f.FileName.Equals(file.FileName)).FileChecksum = newContentChecksum;
                                await _syncTaskDataService.AddOrUpdateTask(task);

                                // Notification
                                _notificationService.NotifyFileUpdated(targetFilePath);
                                _logger.LogInformation($"Sync Task {task.Id}-{task.GistId} - {file.FileName} has been updated.");
                            }
                        }
                    }
                },
                task.UpdatedAt,
                task.GitHubPersonalAccessToken);

            _gistWatchUnsubscriber = _gistWatcherService.Subscribe(gistWatch);
        }

        public virtual void Destroy()
        {
            _gistWatchUnsubscriber.Dispose();
        }

        public virtual string GistId => _syncTask.GistId;
    }
}