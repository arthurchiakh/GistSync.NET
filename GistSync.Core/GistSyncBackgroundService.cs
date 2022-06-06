using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GistSync.Core.Extensions;
using GistSync.Core.Models;
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
        private readonly IDictionary<int, ISyncStrategy> _gistSyncContexts;

        public GistSyncBackgroundService(ILogger<GistSyncBackgroundService> logger, ISyncTaskDataService syncTaskDataService,
            SyncStrategyProvider syncStrategyProvider)
        {
            _gistSyncContexts = new Dictionary<int, ISyncStrategy>();

            _logger = logger;
            _syncTaskDataService = syncTaskDataService;
            _syncStrategyProvider = syncStrategyProvider;
        }

        /// <summary>
        /// Background service start: Create and start all existing sync tasks
        /// </summary>
        /// <param name="stoppingToken">Stopping token</param>
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var syncTasks = await _syncTaskDataService.GetAllTasks();

            foreach (var syncTask in syncTasks.Where(t => t.IsEnabled))
                StartSyncTask(syncTask);

            await stoppingToken;
        }

        /// <summary>
        /// Background service start: Create and start all existing sync tasks
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var context in _gistSyncContexts.Values)
                context.Destroy();

            return base.StopAsync(cancellationToken);
        }

        /// <summary>
        /// Stop and remove sync task by using sync task id
        /// </summary>
        /// <param name="syncTaskId">SyncTaskId</param>
        public void StopSyncTaskById(int syncTaskId)
        {
            if (_gistSyncContexts.ContainsKey(syncTaskId))
            {
                var syncTask = _gistSyncContexts[syncTaskId];
                syncTask.Destroy();
                _gistSyncContexts.Remove(syncTaskId);
                _logger.LogInformation("Stopped sync task successfully: {0}-{1}", syncTaskId, syncTask.GistId);
            }
        }

        /// <summary>
        /// Start sync task
        /// </summary>
        /// <param name="syncTask"></param>
        public void StartSyncTask(SyncTask syncTask)
        {
            if (!_gistSyncContexts.ContainsKey(syncTask.Id))
            {
                var strategy = _syncStrategyProvider.Provide(syncTask.SyncMode);
                strategy.Setup(syncTask);

                _gistSyncContexts[syncTask.Id] = strategy;

                _logger.LogInformation("Started sync task successfully: {0}-{1}", syncTask.Id, syncTask.GistId);
            }
        }
    }
}