using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using GistSync.Core.Models;
using GistSync.Core.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Timer = System.Timers.Timer;

namespace GistSync.Core.Services
{
    public class GistWatcherService : IGistWatcherService
    {
        private readonly IGitHubApiService _gitHubApiService;
        private readonly IConfiguration _config;
        private readonly Timer _timer;
        private readonly SemaphoreSlim _semaphoreSlim;
        private readonly IDictionary<string, GistWatch> _watches;
        private readonly IDictionary<string, int> _watchCounter;
        private readonly object _watchesLock;

        public GistWatcherService(IGitHubApiService gitHubApiService, IConfiguration config)
        {
            _gitHubApiService = gitHubApiService;
            _config = config;

            _semaphoreSlim = new SemaphoreSlim(_config.GetValue("Gist:MaxConcurrentGistStatusCheck", 5));
            _watches = new Dictionary<string, GistWatch>();
            _watchCounter = new Dictionary<string, int>();
            _watchesLock = new object();

            _timer = new Timer
            {
                Enabled = true,
                AutoReset = false
            };
            _timer.Elapsed += OnTimerOnElapsed;
            _timer.Start();
        }

        ~GistWatcherService()
        {
            Dispose();
        }

        private void OnTimerOnElapsed(object sender, ElapsedEventArgs args)
        {
            lock (_watchesLock)
            {
                Task.WhenAll(_watches.Values.Select(PerformGistWatch)).Wait();

                _timer.Interval = TimeSpan.FromSeconds(_config.GetValue("Gist:StatusRefreshIntervalSeconds", 300)).TotalMilliseconds;
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