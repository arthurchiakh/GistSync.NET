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
        private readonly ConcurrentDictionary<int, GistWatch> _watches;

        private PeriodicTimer _periodicTimer;
        private SemaphoreSlim _semaphoreSlim;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        private Task? _pollingTask;

        public GistWatcherService(IGitHubApiService gitHubApiService, IConfiguration config,
            ILogger<GistWatcherService> logger)
        {
            _gitHubApiService = gitHubApiService;
            _config = config;
            _logger = logger;
            _watches = new ConcurrentDictionary<int, GistWatch>();

            StartPollingTask();
        }

        ~GistWatcherService()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (!_cancellationTokenSource.IsCancellationRequested) _cancellationTokenSource.Cancel();
        }

        public void ReloadSettings()
        {
            StopPollingTask();
            StartPollingTask();
        }

        public void OverrideGistUpdatedAtUtc(int syncTaskId, DateTime dateTimeUtc)
        {
            _watches[syncTaskId].UpdatedAtUtc = dateTimeUtc;
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

        private void StartPollingTask()
        {
            _logger.LogDebug("Starting polling task.");

            _periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(_config.GetOrSet("StatusRefreshIntervalSeconds", 300)));
            _semaphoreSlim = new SemaphoreSlim(_config.GetOrSet("MaxConcurrentGistStatusCheck", 5));
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _pollingTask = Task.Factory.StartNew(Polling, _cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            _logger.LogDebug("Started polling task.");
        }

        private void StopPollingTask()
        {
            _logger.LogDebug("Stopping polling task.");

            if (_pollingTask == null || _cancellationTokenSource is not { IsCancellationRequested: false }) return;

            _cancellationTokenSource.Cancel();
            _pollingTask = null;
            _periodicTimer.Dispose();

            _logger.LogDebug("Stopped polling task.");
        }

        private async void Polling()
        {
            try
            {
                while (await _periodicTimer.WaitForNextTickAsync(_cancellationToken))
                {
                    if (!_watches.Any()) continue; // Skip if no watches

                    _cancellationToken.ThrowIfCancellationRequested();

                    await Task.WhenAll(_watches.Values.Select(WatchGist));
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogDebug("Polling has been canceled.");
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
                _logger.LogError(ex, "Failed to get Gist info. {0}", gistWatch.GistId);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
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