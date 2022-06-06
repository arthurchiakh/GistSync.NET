﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using GistSync.Core.Models;
using GistSync.Core.Services.Contracts;
using GistSync.Core.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Timer = System.Timers.Timer;

namespace GistSync.Core.Services
{
    public class GistWatcherService : IGistWatcherService
    {
        private readonly IGitHubApiService _gitHubApiService;
        private readonly ILogger<GistWatcherService> _logger;
        private readonly IConfiguration _config;
        private readonly Timer _timer;
        private SemaphoreSlim _semaphoreSlim;
        private readonly IDictionary<string, GistWatch> _watches;
        private readonly IDictionary<string, int> _watchCounter;
        private readonly object _watchesLock;

        public GistWatcherService(IGitHubApiService gitHubApiService, IConfiguration config, 
            ILogger<GistWatcherService> logger)
        {
            _gitHubApiService = gitHubApiService;
            _config = config;
            _logger = logger;

            _semaphoreSlim = new SemaphoreSlim(_config.GetOrSet("MaxConcurrentGistStatusCheck", 5));
            _watches = new Dictionary<string, GistWatch>();
            _watchCounter = new Dictionary<string, int>();
            _watchesLock = new object();

            _timer = new Timer
            {
                Enabled = true,
                AutoReset = false,
                Interval = TimeSpan.FromSeconds(_config.GetOrSet("StatusRefreshIntervalSeconds", 300)).TotalMilliseconds
            };
            _timer.Elapsed += OnTimerOnElapsed;
            _timer.Start();
        }

        ~GistWatcherService()
        {
            Dispose();
        }

        private void OnTimerOnElapsed(object? sender, ElapsedEventArgs args)
        {
            lock (_watchesLock)
            {
                Task.WhenAll(_watches.Values.Select(PerformGistWatch)).Wait();

                _timer.Interval = TimeSpan.FromSeconds(_config.GetOrSet("StatusRefreshIntervalSeconds", 300)).TotalMilliseconds;
                _semaphoreSlim = new SemaphoreSlim(_config.GetOrSet("MaxConcurrentGistStatusCheck", 5));
                _timer.Start();
            }
        }

        private async Task PerformGistWatch(GistWatch gistWatch)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                var gist = await _gitHubApiService.Gist(gistWatch.GistId, gistWatch.PersonalAccessToken);
                if (gistWatch.UpdatedAtUtc != gist.UpdatedAt)
                {
                    gistWatch.Files = gist.Files.Select(kv => kv.Value).ToArray();
                    gistWatch.UpdatedAtUtc = gist.UpdatedAt;
                    gistWatch.TriggerGistUpdatedEvent();
                }
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
            lock (_watchesLock)
            {
                // Set gist watch
                if (!_watches.ContainsKey(gistWatch.GistId))
                    _watches[gistWatch.GistId] = gistWatch;

                // Set reference counter
                _watchCounter[gistWatch.GistId] = _watchCounter.ContainsKey(gistWatch.GistId) ? _watchCounter[gistWatch.GistId]++ : 1;
                
                // Perform sync right away in another thread.
                Task.Run(() => { PerformGistWatch(gistWatch).Wait(); });

                return new Unsubscriber(_watches, _watchCounter, _watchesLock, gistWatch);
            }
        }

        public void OverrideGistUpdatedAtUtc(string gistId, DateTime dateTimeUtc)
        {
            lock (_watchesLock)
                _watches[gistId].UpdatedAtUtc = dateTimeUtc;
        }

        public void Dispose()
        {
            _timer.Stop();
            _timer.Dispose();
        }

        private class Unsubscriber : IDisposable
        {
            private readonly IDictionary<string, GistWatch> _watches;
            private readonly IDictionary<string, int> _watchCounter;
            private readonly GistWatch _gistWatch;
            private readonly object _watchesLock;

            public Unsubscriber(IDictionary<string, GistWatch> watches, IDictionary<string, int> watchCounter, object watchesLock, GistWatch gistWatch)
            {
                _watches = watches;
                _watchCounter = watchCounter;
                _watchesLock = watchesLock;
                _gistWatch = gistWatch;
            }

            public void Dispose()
            {
                lock (_watchesLock)
                    if (_watchCounter.ContainsKey(_gistWatch.GistId))
                    {
                        if (_watchCounter[_gistWatch.GistId] > 1)
                            _watchCounter[_gistWatch.GistId] -= 1; // Minus counter by 1
                        else
                        {
                            _watchCounter.Remove(_gistWatch.GistId);
                            _watches.Remove(_gistWatch.GistId);
                        }
                    }
                    else // Remove watch directly
                        _watches.Remove(_gistWatch.GistId);
            }
        }
    }
}