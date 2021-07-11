using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Net;
using System.Net.NetworkInformation;
using GistSync.Core.Extensions;
using GistSync.Core.Factories.Contracts;
using GistSync.Core.Models;
using GistSync.Core.Models.GitHub;
using GistSync.Core.Services.Contracts;
using GistSync.Core.Strategies.Contracts;
using Microsoft.Extensions.Hosting;
using File = GistSync.Core.Models.GitHub.File;

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

        internal TwoWaySyncStrategy(IFileSystem fileSystem, 
            IFileWatchFactory fileWatchFactory, IFileWatcherService fileWatcherService, 
            ActiveSyncStrategy activeSyncStrategy, IGitHubApiService gitHubApiService)
        {
            _fileSystem = fileSystem;
            _fileWatchFactory = fileWatchFactory;
            _fileWatcherService = fileWatcherService;
            _activeSyncStrategy = activeSyncStrategy;
            _gitHubApiService = gitHubApiService;
        }

        public TwoWaySyncStrategy(IFileWatchFactory fileWatchFactory, IFileWatcherService fileWatcherService, 
            ActiveSyncStrategy activeSyncStrategy, IGitHubApiService gitHubApiService)
            : this(new FileSystem(), fileWatchFactory, fileWatcherService, activeSyncStrategy, gitHubApiService)
        {
        }

        public void Setup(SyncTask task)
        {
            // Setup for ActiveSync
            _activeSyncStrategy.Setup(task);

            var fileWatch = _fileWatchFactory.Create(task.MappedLocalFilePath);

            fileWatch.FileContentChangedEvent += async (sender, args) =>
            {
                var updatedGist = _gitHubApiService.PatchGist(task.GistId, new GistPatch
                {
                    Files = new Dictionary<string, FilePatch>
                    {
                        {
                            task.GistFileName, new FilePatch
                            {
                                FileName = task.GistFileName,
                                Content = await _fileSystem.File.ReadAllTextInReadOnlyModeAsync(args.FilePath)
                            }
                        }
                    }
                }, task.GitHubPersonalAccessToken);
            };

            _fileWatcherService.AddWatch(fileWatch);
        }
    }
}