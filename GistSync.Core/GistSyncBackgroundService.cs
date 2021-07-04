using System.Threading;
using System.Threading.Tasks;
using GistSync.Core.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GistSync.Core
{
    public class GistSyncBackgroundService : BackgroundService
    {
        private readonly ILogger _logger;

        public GistSyncBackgroundService(ILogger<GistSyncBackgroundService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // TODO: Register tasks and watcher

            await stoppingToken;
        }
    }
}