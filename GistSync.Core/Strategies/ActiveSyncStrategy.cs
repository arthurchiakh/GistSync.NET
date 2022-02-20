using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using GistSync.Core.Factories.Contracts;
using GistSync.Core.Models;
using GistSync.Core.Services.Contracts;
using GistSync.Core.Strategies.Contracts;

namespace GistSync.Core.Strategies
{
    [RegisterForSyncStrategy(SyncModeTypes.ActiveSync)]
    public class ActiveSyncStrategy : ISyncStrategy
    {
        private readonly IFileSystem _fileSystem;
        private readonly IGistWatchFactory _gistWatchFactory;
        private readonly IGistWatcherService _gistWatcherService;
        private readonly IFileChecksumService _fileChecksumService;
        private readonly ISyncTaskDataService _syncTaskDataService;
        private readonly ISynchronizedFileAccessService _synchronizedFileAccessService;
        private readonly IGitHubApiService _gitHubApiService;
        private readonly INotificationService _notificationService;
        private IDisposable _gistWatchUnsubscriber;

        internal ActiveSyncStrategy(IFileSystem fileSystem,
                    IGistWatchFactory gistWatchFactory, IGistWatcherService gistWatcherService,
                    IFileChecksumService fileChecksumService, ISyncTaskDataService syncTaskDataService,
                    ISynchronizedFileAccessService synchronizedFileAccessService, IGitHubApiService gitHubApiService,
                    INotificationService notificationService)
        {
            _gistWatchFactory = gistWatchFactory;
            _gistWatcherService = gistWatcherService;
            _fileChecksumService = fileChecksumService;
            _syncTaskDataService = syncTaskDataService;
            _synchronizedFileAccessService = synchronizedFileAccessService;
            _gitHubApiService = gitHubApiService;
            _notificationService = notificationService;
            _fileSystem = fileSystem;
        }

        public ActiveSyncStrategy(IGistWatchFactory gistWatchFactory, IGistWatcherService gistWatcherService,
                                    IFileChecksumService fileShChecksumService, ISyncTaskDataService syncTaskDataService,
                                    ISynchronizedFileAccessService synchronizedFileAccessService, IGitHubApiService gitHubApiService,
                                    INotificationService notificationService)
            : this(new FileSystem(), gistWatchFactory, gistWatcherService, fileShChecksumService,
                syncTaskDataService, synchronizedFileAccessService, gitHubApiService, notificationService)
        {
        }

        public void Setup(SyncTask task)
        {
            // Create watch object
            var gistWatch = _gistWatchFactory.Create(task.GistId,
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

                        // If file already exists then need to verify checksum.
                        // Because, the updated time could be changed by updating other files and new comment in the gist
                        if (_fileSystem.File.Exists(task.Directory))
                        {
                            var existingContentChecksum = _fileChecksumService.ComputeChecksumByFilePath(task.Directory);
                            // No need to update if content is the same
                            if (newContentChecksum.Equals(existingContentChecksum, StringComparison.OrdinalIgnoreCase))
                            {
                                // Update UpdatedAtUtc datetime
                                task.UpdatedAt = args.UpdatedAtUtc;
                                await _syncTaskDataService.AddOrUpdateTask(task);
                                return;
                            }
                        }

                        var targetFilePath = Path.Combine(task.Directory, file.FileName);

                        // Write to the file
                        await _synchronizedFileAccessService.SynchronizedWriteStream(targetFilePath, FileMode.Create,
                            async stream => { await stream.WriteAsync(Encoding.UTF8.GetBytes(newContent)); }
                        );

                        // Update UpdatedAtUtc datetime
                        task.UpdatedAt = args.UpdatedAtUtc;
                        task.Files.First(f => f.FileName.Equals(file.FileName)).FileChecksum = newContentChecksum;
                        await _syncTaskDataService.AddOrUpdateTask(task);

                        // Notification
                        _notificationService.NotifyFileUpdated(task.Directory);
                    }
                },
                task.UpdatedAt,
                task.GitHubPersonalAccessToken);

            _gistWatchUnsubscriber = _gistWatcherService.Subscribe(gistWatch);
        }

        public void Destroy()
        {
            _gistWatchUnsubscriber.Dispose();
        }
    }
}