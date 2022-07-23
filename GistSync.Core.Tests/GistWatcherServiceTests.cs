using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
        private const string GistId = "db136774b432d328fcc041ea0ea1f88a";

        [Test, Timeout(5000)]
        public async Task GistWatcherService_LaterUpdatedDate_ExpectTriggerEvent()
        {
            // Mock GitHubApiService
            var gitHubApiServiceMock = new Mock<IGitHubApiService>().As<IGitHubApiService>();
            gitHubApiServiceMock.Setup(s => s.Gist(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()).Result)
                .Returns(new Gist
                {
                    Id = GistId,
                    UpdatedAt = new Lazy<DateTime>(() => DateTime.UtcNow).Value,
                    Files = new Dictionary<string, File>()
                });

            // Mock Configuration
            var config = ConfigurationUtil.MockConfiguration();

            // Mock Logger
            var logger = new Mock<ILogger<GistWatcherService>>().Object;

            // Mock SyncTaskDataServiceM
            var syncTaskDataServiceMock = new Mock<ISyncTaskDataService>();
            syncTaskDataServiceMock.Setup(s => s.GetTask(It.IsAny<int>()))
                .ReturnsAsync(new SyncTask { Id = 1, GistId = "db136774b432d328fcc041ea0ea1f88a" });
            var syncTaskDataService = syncTaskDataServiceMock.Object;

            // Create GistWatcherService 
            var gistWatcherService = new GistWatcherService(gitHubApiServiceMock.Object, config, logger, syncTaskDataService);

            var triggerFlag = false;

            // Add watch
            gistWatcherService.Subscribe(1, _ => triggerFlag = true);

            // The actual work done in different thread, so we wait a bit for the thread to finish the job
            await Task.Delay(100);

            Assert.IsTrue(triggerFlag);
        }
    }
}