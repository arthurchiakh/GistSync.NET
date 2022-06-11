using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GistSync.Core.Factories;
using GistSync.Core.Models;
using GistSync.Core.Models.GitHub;
using GistSync.Core.Services;
using GistSync.Core.Services.Contracts;
using GistSync.Core.Tests.Utils;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace GistSync.Core.Tests
{
    public class GistWatcherServiceTests
    {
        private string _gistId = "db136774b432d328fcc041ea0ea1f88a";
        private IGistWatcherService _gistWatcherService;

        [Test, Timeout(5000)]
        public async Task GistWatcherService_LaterUpdatedDate_ExpectTriggerEvent()
        {
            // Mock GitHubApiService
            var gitHubApiServiceMock = new Mock<IGitHubApiService>().As<IGitHubApiService>();
            gitHubApiServiceMock.Setup(s => s.Gist(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()).Result)
                .Returns(new Gist
                {
                    Id = _gistId,
                    UpdatedAt = new Lazy<DateTime>(() => DateTime.UtcNow).Value,
                    Files = new Dictionary<string, File>()
                });

            // Mock Configuration
            var config = ConfigurationUtil.MockConfiguration();

            // Mock Logger
            var logger = new Mock<ILogger<GistWatcherService>>().Object;

            // Create service
            _gistWatcherService = new GistWatcherService(gitHubApiServiceMock.Object, config, logger);

            var triggerFlag = false;

            // Create gist watch
            var gistWatchFactory = new GistWatchFactory();
            var gistWatch = gistWatchFactory.Create(new SyncTask { Id = 1, GistId = _gistId }, (sender, args) => triggerFlag = true, DateTime.UtcNow);

            // Add watch
            _gistWatcherService.Subscribe(gistWatch);

            // The actual work done in different thread, so we wait a bit for the thread to finish the job
            await Task.Delay(100);

            Assert.IsTrue(triggerFlag);
        }
    }
}