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
    [RegisterForSyncStrategy(SyncStrategyTypes.ActiveSync)]
    public class ActiveSyncStrategy : ISyncStrategy
    {
        private readonly IFileSystem _fileSystem;
        private readonly IGistWatchFactory _gistWatchFactory;
        private readonly IGistWatcherService _gistWatcherService;
        private readonly IFileChecksumService _fileChecksumService;
        private readonly ISyncTaskDataService _syncTaskDataService;
        private readonly ISynchronizedFileAccessService _synchronizedFileAccessService;
        private readonly IGitHubApiService _gitHubApiService;
        private IDisposable _gistWatchUnsubscriber;

        internal ActiveSyncStrategy(IFileSystem fileSystem,
                    IGistWatchFactory gistWatchFactory, IGistWatcherService gistWatcherService,
                    IFileChecksumService fileChecksumService, ISyncTaskDataService syncTaskDataService,
                    ISynchronizedFileAccessService synchronizedFileAccessService, IGitHubApiService gitHubApiService)
        {
            _gistWatchFactory = gistWatchFactory;
            _gistWatcherService = gistWatcherService;
            _fileChecksumService = fileChecksumService;
            _syncTaskDataService = syncTaskDataService;
            _synchronizedFileAccessService = synchronizedFileAccessService;
            _gitHubApiService = gitHubApiService;
            _fileSystem = fileSystem;
        }

        public ActiveSyncStrategy(IGistWatchFactory gistWatchFactory, IGistWatcherService gistWatcherService,
                                    IFileChecksumService fileShChecksumService, ISyncTaskDataService syncTaskDataService,
                                    ISynchronizedFileAccessService synchronizedFileAccessService, IGitHubApiService gitHubApiService)
            : this(new FileSystem(), gistWatchFactory, gistWatcherService, fileShChecksumService,
                syncTaskDataService, synchronizedFileAccessService, gitHubApiService)
        {
        }

        public void Setup(SyncTask task)
        {
            // Create watch object
            var gistWatch = _gistWatchFactory.Create(task.GistId,
                async (sender, args) =>
                {
                    var file = args.Files.FirstOrDefault(f => f.FileName.Equals(task.GistFileName));
                    // Exit if the file not found in gist
                    // Probably the file has been renamed 
                    if (file == null) return;

                    string newContent;
                    if (!file.Truncated.GetValueOrDefault(false))
                        newContent = file.Content;
                    else
                        newContent = await _gitHubApiService.GetFileContentByUrl(file.RawUrl);

                    var newContentChecksum = await _fileChecksumService.ComputeChecksumByFileContentAsync(newContent);

                    // If file already exists then need to verify checksum.
                    // Because, the updated time could be changed by updating other files and new comment in the gist
                    if (_fileSystem.File.Exists(task.MappedLocalFilePath))
                    {
                        var existingContentChecksum = _fileChecksumService.ComputeChecksumByFilePath(task.MappedLocalFilePath);
                        // No need to update if content is the same
                        if (newContentChecksum.Equals(existingContentChecksum, StringComparison.OrdinalIgnoreCase))
                        {
                            // Update UpdatedAtUtc datetime
                            task.GistUpdatedAt = args.UpdatedAtUtc;
                            _syncTaskDataService.AddOrUpdateTask(task);
                            return;
                        }
                    }

                    // Write to the file
                    await _synchronizedFileAccessService.SynchronizedWriteStream(task.MappedLocalFilePath, FileMode.Create,
                        async stream => { await stream.WriteAsync(Encoding.UTF8.GetBytes(newContent)); }
                    );

                    // Update UpdatedAtUtc datetime
                    task.GistUpdatedAt = args.UpdatedAtUtc;
                    task.FileChecksum = newContentChecksum;
                    _syncTaskDataService.AddOrUpdateTask(task);
                },
                task.GistUpdatedAt,
                task.GitHubPersonalAccessToken);

            _gistWatchUnsubscriber = _gistWatcherService.Subscribe(gistWatch);
        }

        public void Destroy()
        {
            _gistWatchUnsubscriber.Dispose();
        }
    }
}