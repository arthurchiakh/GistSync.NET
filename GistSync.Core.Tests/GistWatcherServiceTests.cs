using System;
using System.Threading.Tasks;
using GistSync.Core.Factories;
using GistSync.Core.Models.GitHub;
using GistSync.Core.Models.Settings;
using GistSync.Core.Services;
using GistSync.Core.Services.Contracts;
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
            var gitHubApiServiceMock = new Mock<IGitHubApiService>().As<IGitHubApiService>();
            gitHubApiServiceMock.Setup(s => s.Gist(It.IsAny<string>(), It.IsAny<string>()).Result)
                .Returns(new Gist()
                {
                    id = _gistId,
                    UpdatedAt = new Lazy<DateTime>(() => DateTime.UtcNow).Value
                });

            var appSettingService = new Mock<AppSettings>().As<IAppSettingService>();
            appSettingService.Setup(s => s.Settings())
                .Returns(new AppSettings
                {
                    GitHub = new GitHub
                    {
                        MaxConcurrentGistStatusCheck = 1,
                        StatusRefreshIntervalSeconds = 1
                    }
                });

            _gistWatcherService = new Mock<GistWatcherService>(gitHubApiServiceMock.Object, appSettingService.Object) { CallBase = true }
                .As<IGistWatcherService>().Object;

            var triggerFlag = false;

            // Create gist watch
            var gistWatchFactory = new GistWatchFactory();
            var gistWatch = gistWatchFactory.Create(_gistId);
            gistWatch.UpdatedAtUtc = DateTime.UtcNow;
            gistWatch.GistUpdatedEvent += gistWatch => triggerFlag = true;

            // Add watch
            _gistWatcherService.AddWatch(gistWatch);

            // Give buffer time to let timer to pick the job
            await Task.Delay(200);

            Assert.IsTrue(triggerFlag);
        }
    }
}