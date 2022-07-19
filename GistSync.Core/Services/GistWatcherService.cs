using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GistSync.Core.Models.GitHub;
using GistSync.Core.Services.Contracts;
using GistSync.Core.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GistSync.Core.Services
{
    public class GistWatcherService : IGistWatcherService
    {
        private readonly IGitHubApiService _gitHubApiService;
        private readonly ILogger<GistWatcherService> _logger;
        private readonly IConfiguration _config;
        private readonly ConcurrentDictionary<int, Action<Gist>> _items;
        private readonly ISyncTaskDataService _syncTaskDataService;

        private PeriodicTimer? _periodicTimer;
        private SemaphoreSlim? _semaphoreSlim;
        private CancellationTokenSource? _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        private Task? _pollingTask;

        public GistWatcherService(IGitHubApiService gitHubApiService, IConfiguration config,
            ILogger<GistWatcherService> logger, ISyncTaskDataService syncTaskDataService)
        {
            _gitHubApiService = gitHubApiService;
            _config = config;
            _logger = logger;
            _syncTaskDataService = syncTaskDataService;
            _items = new ConcurrentDictionary<int, Action<Gist>>();

            StartPollingTask();
        }

        ~GistWatcherService()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_cancellationTokenSource is {IsCancellationRequested: false}) _cancellationTokenSource.Cancel();
        }

        public void ReloadConfigurationSettings()
        {
            StopPollingTask();
            StartPollingTask();
        }

        public IDisposable Subscribe(int syncTaskId, Action<Gist> gistUpdatedHandler)
        {
            // Perform sync right away in another thread.
            Task.Run(async () =>
            {
                await WatchGist(syncTaskId, gistUpdatedHandler);
                _items.TryAdd(syncTaskId, gistUpdatedHandler);
            }, _cancellationToken);

            return new Unsubscriber(_items, syncTaskId);
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
            _periodicTimer?.Dispose();

            _logger.LogDebug("Stopped polling task.");
        }

        private async void Polling()
        {
            try
            {
                while (await _periodicTimer!.WaitForNextTickAsync(_cancellationToken))
                {
                    if (!_items.Any()) continue; // Skip if no watches

                    _cancellationToken.ThrowIfCancellationRequested();

                    await Task.WhenAll(_items.Select(kv => WatchGist(kv.Key, kv.Value)));
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogDebug("Polling has been canceled.");
            }
        }

        private async Task WatchGist(int syncTaskId, Action<Gist> gistUpdatedHandler)
        {
            await _semaphoreSlim!.WaitAsync(_cancellationToken);

            var syncTask = await _syncTaskDataService.GetTask(syncTaskId);

            try
            {
                var gist = await _gitHubApiService.Gist(syncTask.GistId, syncTask.GitHubPersonalAccessToken!, _cancellationToken);
                gistUpdatedHandler.Invoke(gist);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get Gist info. {0}", syncTask.GistId);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        private class Unsubscriber : IDisposable
        {
            private readonly ConcurrentDictionary<int, Action<Gist>> _items;
            private readonly int _syncTaskId;

            public Unsubscriber(ConcurrentDictionary<int, Action<Gist>> items, int syncTaskId)
            {
                _items = items;
                _syncTaskId = syncTaskId;
            }

            public void Dispose()
            {
                _items.TryRemove(_syncTaskId, out _);
            }
        }
    }
}