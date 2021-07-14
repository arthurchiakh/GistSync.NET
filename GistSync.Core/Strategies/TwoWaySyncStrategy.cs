using System.Collections.Generic;
using System.IO.Abstractions;
using GistSync.Core.Factories.Contracts;
using GistSync.Core.Models;
using GistSync.Core.Models.GitHub;
using GistSync.Core.Services.Contracts;
using GistSync.Core.Strategies.Contracts;

namespace GistSync.Core.Strategies
{
    [RegisterForSyncStrategy(SyncStrategyTypes.TwoWaySync)]
    public class TwoWaySyncStrategy : ISyncStrategy
    {
        private readonly IFileSystem _fileSystem;
        private readonly IFileWatchFactory _fileWatchFactory;
        private readonly IFileWatcherService _fileWatcherService;
        private readonly ActiveSyncStrategy _activeSyncStrategy;
        private readonly IGitHubApiService _gitHubApiService;
        private readonly ISynchronizedFileAccessService _synchronizedFileAccessService;
        private readonly IGistWatcherService _gistWatcherService;

        internal TwoWaySyncStrategy(IFileSystem fileSystem,
            IFileWatchFactory fileWatchFactory, IFileWatcherService fileWatcherService,
            ActiveSyncStrategy activeSyncStrategy, IGitHubApiService gitHubApiService,
            ISynchronizedFileAccessService synchronizedFileAccessService, IGistWatcherService gistWatcherService)
        {
            _fileSystem = fileSystem;
            _fileWatchFactory = fileWatchFactory;
            _fileWatcherService = fileWatcherService;
            _activeSyncStrategy = activeSyncStrategy;
            _gitHubApiService = gitHubApiService;
            _synchronizedFileAccessService = synchronizedFileAccessService;
            _gistWatcherService = gistWatcherService;
        }

        public TwoWaySyncStrategy(IFileWatchFactory fileWatchFactory, IFileWatcherService fileWatcherService,
            ActiveSyncStrategy activeSyncStrategy, IGitHubApiService gitHubApiService,
            ISynchronizedFileAccessService synchronizedFileAccessService, IGistWatcherService gistWatcherService)
            : this(new FileSystem(), fileWatchFactory, fileWatcherService, activeSyncStrategy,
                gitHubApiService, synchronizedFileAccessService, gistWatcherService)
        {
        }

        public void Setup(SyncTask task)
        {
            // Setup for ActiveSync
            _activeSyncStrategy.Setup(task);

            var fileWatch = _fileWatchFactory.Create(task.MappedLocalFilePath);

            fileWatch.FileContentChangedEvent += async (sender, args) =>
            {
                var updatedGist = await _gitHubApiService.PatchGist(task.GistId, new GistPatch
                {
                    Files = new Dictionary<string, FilePatch>
                    {
                        {
                            task.GistFileName, new FilePatch
                            {
                                FileName = task.GistFileName,
                                Content = _synchronizedFileAccessService.SynchronizedReadAllText(args.FilePath)
                            }
                        }
                    }
                }, task.GitHubPersonalAccessToken);

                // Prevent back sync occurs
                _gistWatcherService.SetGistUpdatedAtUtc(updatedGist.Id, updatedGist.UpdatedAt.Value);
            };

            _fileWatcherService.AddWatch(fileWatch);
        }
    }
}