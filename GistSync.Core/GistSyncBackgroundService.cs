using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GistSync.Core.Extensions;
using GistSync.Core.Services.Contracts;
using GistSync.Core.Strategies;
using GistSync.Core.Strategies.Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GistSync.Core
{
    public class GistSyncBackgroundService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly ISyncTaskDataService _syncTaskDataService;
        private readonly SyncStrategyProvider _syncStrategyProvider;
        private readonly IDictionary<string, ISyncStrategy> _gistSyncContexts;

        public GistSyncBackgroundService(ILogger<GistSyncBackgroundService> logger, ISyncTaskDataService syncTaskDataService,
            SyncStrategyProvider syncStrategyProvider)
        {
            _gistSyncContexts = new Dictionary<string, ISyncStrategy>();

            _logger = logger;
            _syncTaskDataService = syncTaskDataService;
            _syncStrategyProvider = syncStrategyProvider;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var tasks = _syncTaskDataService.GetAllTasks();

            foreach (var task in tasks)
            {
                var strategy = _syncStrategyProvider.Provide(task.SyncMode);
                strategy.Setup(task);

                _gistSyncContexts[task.GistId] = strategy;
            }

            await stoppingToken;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            foreach(var context in _gistSyncContexts.Values)
                context.Destroy();

            return base.StopAsync(cancellationToken);
        }
    }
}