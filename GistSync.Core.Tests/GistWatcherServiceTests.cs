using System;
using System.Threading;
using System.Threading.Tasks;
using GistSync.Core.Factories;
using GistSync.Core.Models.GitHub;
using GistSync.Core.Services;
using GistSync.Core.Services.Contracts;
using GistSync.Core.Tests.Utils;
using Moq;
using NUnit.Framework;

namespace GistSync.Core.Tests
{
    public class GistWatcherServiceTests
    {
        private string _gistId = "db136774b432d328fcc041ea0ea1f88a";
        private IGistWatcherService _gistWatcherService;

        [TestCase]
        public async Task GistWatcherService_LaterUpdatedDate_ExpectTriggerEvent()
        {
            // Mock GitHubApiService
            var gitHubApiServiceMock = new Mock<IGitHubApiService>().As<IGitHubApiService>();
            gitHubApiServiceMock.Setup(s => s.Gist(It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None).Result)
                .Returns(new Gist
                {
                    id = _gistId,
                    UpdatedAt = new Lazy<DateTime>(() => DateTime.UtcNow).Value
                });

            // Mock Configuration
            var config = ConfigurationUtil.MockConfiguration();

            // Create service
            _gistWatcherService = new GistWatcherService(gitHubApiServiceMock.Object, config);

            var triggerFlag = false;

            // Create gist watch
            var gistWatchFactory = new GistWatchFactory();
            var gistWatch = gistWatchFactory.Create(_gistId);
            gistWatch.UpdatedAtUtc = DateTime.UtcNow;
            gistWatch.GistUpdatedEvent += (sender, args) => triggerFlag = true;

            // Add watch
            _gistWatcherService.AddWatch(gistWatch);

            // Give buffer time to let timer to pick the job
            await Task.Delay(200);

            Assert.IsTrue(triggerFlag);
        }
    }
}