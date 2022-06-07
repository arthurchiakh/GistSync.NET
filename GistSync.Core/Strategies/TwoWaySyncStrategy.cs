using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using GistSync.Core.Factories.Contracts;
using GistSync.Core.Models;
using GistSync.Core.Models.GitHub;
using GistSync.Core.Services.Contracts;
using Microsoft.Extensions.Logging;

namespace GistSync.Core.Strategies
{
    [RegisterForSyncStrategy(SyncModeTypes.TwoWaySync)]
    public class TwoWaySyncStrategy : ActiveSyncStrategy
    {
        private readonly IFileWatchFactory _fileWatchFactory;
        private readonly IFileWatcherService _fileWatcherService;
        private IDisposable _fileWatchUnsubscriber;
        private readonly ILogger _logger;

        internal TwoWaySyncStrategy(IFileSystem fileSystem,
            IFileWatchFactory fileWatchFactory, IFileWatcherService fileWatcherService,
            IGitHubApiService gitHubApiService,
            ISynchronizedFileAccessService synchronizedFileAccessService, IGistWatcherService gistWatcherService,
            IFileChecksumService fileChecksumService, ISyncTaskDataService syncTaskDataService,
            INotificationService notificationService, IGistWatchFactory gistWatchFactory,
            ILoggerFactory loggerFactory) : 
            base(fileSystem, gistWatchFactory, gistWatcherService, fileChecksumService,
                syncTaskDataService, synchronizedFileAccessService, gitHubApiService, notificationService,
                loggerFactory)
        {
            _fileWatchFactory = fileWatchFactory;
            _fileWatcherService = fileWatcherService;

            _logger = _loggerFactory.CreateLogger<TwoWaySyncStrategy>();
        }

        public TwoWaySyncStrategy(IFileWatchFactory fileWatchFactory, IFileWatcherService fileWatcherService,
            IGitHubApiService gitHubApiService,
            ISynchronizedFileAccessService synchronizedFileAccessService, IGistWatcherService gistWatcherService,
            IFileChecksumService fileChecksumService, ISyncTaskDataService syncTaskDataService,
            INotificationService notificationService, IGistWatchFactory gistWatchFactory, ILoggerFactory loggerFactory)
            : this(new FileSystem(), fileWatchFactory, fileWatcherService, 
                gitHubApiService, synchronizedFileAccessService, gistWatcherService, fileChecksumService,
                syncTaskDataService, notificationService, gistWatchFactory, loggerFactory)
        {
        }

        public void Setup(SyncTask task)
        {
            // Setup for ActiveSync
            base.Setup(task);

            // Setup file watch
            foreach (var file in task.Files)
            {
                var fileWatch = _fileWatchFactory.Create(Path.Combine(task.Directory, file.FileName),
                    file.FileChecksum!,
                    async (sender, args) =>
                    {
                        var content = _synchronizedFileAccessService.ReadAllText(args.FilePath);
                        if (string.IsNullOrWhiteSpace(content)) return;

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

                        // Prevent back sync occurs
                        _gistWatcherService.OverrideGistUpdatedAtUtc(updatedGist.Id, updatedGist.UpdatedAt!.Value);

                        // Update UpdatedAtUtc datetime
                        task.UpdatedAt = updatedGist.UpdatedAt;
                        file.FileChecksum = args.Checksum;
                        await _syncTaskDataService.AddOrUpdateTask(task);

                        // Notification
                        _notificationService.NotifyGistUpdated(task.GistId);
                        _logger.LogInformation($"Sync Task {task.Id}-{task.GistId} - Gist {task.GistId} has been updated.");
                    });

                _fileWatchUnsubscriber = _fileWatcherService.Subscribe(fileWatch);
            }
        }

        public override void Destroy()
        {
            base.Destroy();
            _fileWatchUnsubscriber.Dispose();
        }
    }
}