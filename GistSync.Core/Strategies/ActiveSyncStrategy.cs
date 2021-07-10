using System;
using System.IO;
using System.IO.Abstractions;
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
        private readonly IGitHubApiService _gistGitHubApiService;
        private readonly IFileChecksumService _fileChecksumService;
        private readonly ISyncTaskDataService _syncTaskDataService;

        internal ActiveSyncStrategy(IFileSystem fileSystem,
                                    IGistWatchFactory gistWatchFactory,
                                    IGistWatcherService gistWatcherService,
                                    IGitHubApiService gistGitHubApiService,
                                    IFileChecksumService fileChecksumService, ISyncTaskDataService syncTaskDataService)
        {
            _gistWatchFactory = gistWatchFactory;
            _gistWatcherService = gistWatcherService;
            _gistGitHubApiService = gistGitHubApiService;
            _fileChecksumService = fileChecksumService;
            _syncTaskDataService = syncTaskDataService;
            _fileSystem = fileSystem;
        }

        public ActiveSyncStrategy(IGistWatchFactory gistWatchFactory,
                                    IGistWatcherService gistWatcherService,
                                    IGitHubApiService gistGitHubApiService,
                                    IFileChecksumService fileShChecksumService, ISyncTaskDataService syncTaskDataService)
            : this(new FileSystem(), gistWatchFactory, gistWatcherService, gistGitHubApiService, fileShChecksumService, syncTaskDataService)
        {
        }

        public  void Setup(SyncTask task)
        {
            // Create watch object
            var gistWatch = _gistWatchFactory.Create(task.GistId, task.GitHubPersonalAccessToken);

            // Register event
            gistWatch.GistUpdatedEvent += async (sender, args) =>
            {
                // Update UpdatedAtUtc datetime
                task.GistUpdatedAt = args.UpdatedAtUtc;
                await _syncTaskDataService.AddOrUpdateTask(task);

                var gist = await _gistGitHubApiService.Gist(args.GistId);

                // Exit if the file not found in gist
                // Probably the file has been renamed 
                if (!gist.Files.TryGetValue(task.GistFileName, out var file)) return;

                string newContent;
                if (file.Truncated.GetValueOrDefault(false))
                    newContent = file.Content;
                else
                    newContent = await _gistGitHubApiService.GetFileContentByUrl(file.RawUrl);

                // If file already exists then need to verify checksum.
                // Because, the updated time could be changed by updating other files and new comment in the gist
                if (_fileSystem.File.Exists(task.MappedLocalFilePath))
                {
                    var newContentChecksum =
                        await _fileChecksumService.ComputeChecksumByFileContentAsync(newContent);
                    var existingContentChecksum =
                        await _fileChecksumService.ComputeChecksumByFilePathAsync(task.MappedLocalFilePath);

                    if (newContentChecksum.Equals(existingContentChecksum, StringComparison.OrdinalIgnoreCase)) return; // No update if content is the same
                }

                // Write to the file
                await using var fs = _fileSystem.File.Open(task.MappedLocalFilePath, FileMode.Create);
                await fs.WriteAsync(Encoding.UTF8.GetBytes(newContent));
            };

            _gistWatcherService.AddWatch(gistWatch);
        }
    }
}