using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GistSync.Core.Models;
using GistSync.Core.Services.Contracts;
using GistSync.Core.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GistSync.Core.Services
{
    public class GistWatcherService : IGistWatcherService, IDisposable
    {
        private readonly IGitHubApiService _gitHubApiService;
        private readonly ILogger<GistWatcherService> _logger;
        private readonly IConfiguration _config;
        private SemaphoreSlim _semaphoreSlim;
        private readonly ConcurrentDictionary<int, GistWatch> _watches;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly CancellationToken _cancellationToken;

        public GistWatcherService(IGitHubApiService gitHubApiService, IConfiguration config,
            ILogger<GistWatcherService> logger)
        {
            _gitHubApiService = gitHubApiService;
            _config = config;
            _logger = logger;
            _watches = new ConcurrentDictionary<int, GistWatch>();
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _semaphoreSlim = new SemaphoreSlim(_config.GetOrSet("MaxConcurrentGistStatusCheck", 5));

            // Start polling
            Task.Factory.StartNew(PollingThread, TaskCreationOptions.LongRunning);
        }

        ~GistWatcherService()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (!_cancellationTokenSource.IsCancellationRequested) _cancellationTokenSource.Cancel();
        }

        private async void PollingThread()
        {
            DateTime? dateTime = null;

            while (true)
            {
                _cancellationToken.ThrowIfCancellationRequested();
                var interval = TimeSpan.FromSeconds(_config.GetOrSet("StatusRefreshIntervalSeconds", 300));

                if (!dateTime.HasValue ||
                    DateTime.UtcNow >= dateTime.Value + interval
                    )
                {
                    _semaphoreSlim = new SemaphoreSlim(_config.GetOrSet("MaxConcurrentGistStatusCheck", 5));
                    await Task.WhenAll(_watches.Values.Select(WatchGist));
                    dateTime = DateTime.UtcNow;
                }

                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        }

        private async Task WatchGist(GistWatch gistWatch)
        {
            await _semaphoreSlim.WaitAsync(_cancellationToken);

            try
            {
                var gist = await _gitHubApiService.Gist(gistWatch.GistId, gistWatch.PersonalAccessToken, _cancellationToken);
                gistWatch.Files = gist.Files.Select(kv => kv.Value).ToArray();
                gistWatch.UpdatedAtUtc = gist.UpdatedAt;
                gistWatch.TriggerGistUpdatedEvent();
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get Gist info. {0}\n{1}", gistWatch.GistId, ex.Message);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public IDisposable Subscribe(GistWatch gistWatch)
        {
            // Perform sync right away in another thread.
            Task.Run(async () =>
            {
                await WatchGist(gistWatch);
                _watches.TryAdd(gistWatch.SyncTaskId, gistWatch);
            }, _cancellationToken);

            return new Unsubscriber(_watches, gistWatch);
        }

        public void OverrideGistUpdatedAtUtc(int syncTaskId, DateTime dateTimeUtc)
        {
            _watches[syncTaskId].UpdatedAtUtc = dateTimeUtc;
        }

        private class Unsubscriber : IDisposable
        {
            private readonly ConcurrentDictionary<int, GistWatch> _watches;
            private readonly GistWatch _gistWatch;

            public Unsubscriber(ConcurrentDictionary<int, GistWatch> watches, GistWatch gistWatch)
            {
                _watches = watches;
                _gistWatch = gistWatch;
            }

            public void Dispose()
            {
                _watches.TryRemove(_gistWatch.SyncTaskId, out _);
            }
        }
    }
}