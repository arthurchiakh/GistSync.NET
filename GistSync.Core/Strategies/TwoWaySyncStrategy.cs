using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using GistSync.Core.Factories.Contracts;
using GistSync.Core.Models;
using GistSync.Core.Models.GitHub;
using GistSync.Core.Services.Contracts;
using GistSync.Core.Strategies.Contracts;

namespace GistSync.Core.Strategies
{
    [RegisterForSyncStrategy(SyncModeTypes.TwoWaySync)]
    public class TwoWaySyncStrategy : ISyncStrategy
    {
        private readonly IFileSystem _fileSystem;
        private readonly IFileWatchFactory _fileWatchFactory;
        private readonly IFileWatcherService _fileWatcherService;
        private readonly ActiveSyncStrategy _activeSyncStrategy;
        private readonly IGitHubApiService _gitHubApiService;
        private readonly ISynchronizedFileAccessService _synchronizedFileAccessService;
        private readonly IGistWatcherService _gistWatcherService;
        private readonly IFileChecksumService _fileChecksumService;
        private readonly ISyncTaskDataService _syncTaskDataService;
        private readonly INotificationService _notificationService;
        private IDisposable _fileWatchUnsubscriber;

        internal TwoWaySyncStrategy(IFileSystem fileSystem,
            IFileWatchFactory fileWatchFactory, IFileWatcherService fileWatcherService,
            ActiveSyncStrategy activeSyncStrategy, IGitHubApiService gitHubApiService,
            ISynchronizedFileAccessService synchronizedFileAccessService, IGistWatcherService gistWatcherService,
            IFileChecksumService fileChecksumService, ISyncTaskDataService syncTaskDataService,
            INotificationService notificationService)
        {
            _fileSystem = fileSystem;
            _fileWatchFactory = fileWatchFactory;
            _fileWatcherService = fileWatcherService;
            _activeSyncStrategy = activeSyncStrategy;
            _gitHubApiService = gitHubApiService;
            _synchronizedFileAccessService = synchronizedFileAccessService;
            _gistWatcherService = gistWatcherService;
            _fileChecksumService = fileChecksumService;
            _syncTaskDataService = syncTaskDataService;
            _notificationService = notificationService;
        }

        public TwoWaySyncStrategy(IFileWatchFactory fileWatchFactory, IFileWatcherService fileWatcherService,
            ActiveSyncStrategy activeSyncStrategy, IGitHubApiService gitHubApiService,
            ISynchronizedFileAccessService synchronizedFileAccessService, IGistWatcherService gistWatcherService,
            IFileChecksumService fileChecksumService, ISyncTaskDataService syncTaskDataService,
            INotificationService notificationService)
            : this(new FileSystem(), fileWatchFactory, fileWatcherService, activeSyncStrategy,
                gitHubApiService, synchronizedFileAccessService, gistWatcherService, fileChecksumService,
                syncTaskDataService, notificationService)
        {
        }

        public void Setup(SyncTask task)
        {
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
                    });

                _fileWatchUnsubscriber = _fileWatcherService.Subscribe(fileWatch);
            }

            // Setup for ActiveSync
            _activeSyncStrategy.Setup(task);
        }

        public void Destroy()
        {
            _activeSyncStrategy.Destroy();
            _fileWatchUnsubscriber.Dispose();
        }
    }
}