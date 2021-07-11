using System.Threading;
using System.Threading.Tasks;
using GistSync.Core.Extensions;
using GistSync.Core.Services.Contracts;
using GistSync.Core.Strategies;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GistSync.Core
{
    public class GistSyncBackgroundService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly ISyncTaskDataService _syncTaskDataService;
        private readonly SyncStrategyProvider _syncStrategyProvider;

        public GistSyncBackgroundService(ILogger<GistSyncBackgroundService> logger, ISyncTaskDataService syncTaskDataService,
            SyncStrategyProvider syncStrategyProvider)
        {
            _logger = logger;
            _syncTaskDataService = syncTaskDataService;
            _syncStrategyProvider = syncStrategyProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var tasks = syncTaskDataService.GetAllTasks();

            foreach (var task in tasks)
            {
                var strategy = _syncStrategyProvider.Provide(task.SyncStrategyType);
                strategy.Setup(task);
            }

            await stoppingToken;
        }
    }
}