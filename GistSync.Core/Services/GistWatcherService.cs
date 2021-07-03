using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Timers;
using GistSync.Core.Models;
using GistSync.Core.Services.Contracts;
using Timer = System.Timers.Timer;

namespace GistSync.Core.Services
{
    public class GistWatcherService : IGistWatcherService
    {
        private readonly IGitHubApiService _gitHubApiService;
        private readonly IAppSettingService _appSettingService;
        private readonly Timer _timer;
        private readonly SemaphoreSlim _semaphoreSlim;
        private readonly IDictionary<string, IList<GistWatch>> _watches;
        private readonly object _watchesLock;

        public GistWatcherService(IGitHubApiService gitHubApiService, IAppSettingService appSettingService)
        {
            _gitHubApiService = gitHubApiService;
            _appSettingService = appSettingService;

            _semaphoreSlim = new SemaphoreSlim(_appSettingService.Settings().GitHub.MaxConcurrentGistStatusCheck);
            _watches = new Dictionary<string, IList<GistWatch>>();
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
            Debug.WriteLine("Start to call api");

            lock (_watchesLock)
            {
                _watches.Values.AsParallel().ForAll(async w =>
                {
                    await _semaphoreSlim.WaitAsync();


                    foreach (var watch in w)
                    {
                        Debug.WriteLine($"Calling api for {watch.GistId}");

                        var gist = await _gitHubApiService.Gist(watch.GistId, watch.PersonalAccessToken);
                        if (gist.UpdatedAt.HasValue)
                            watch.UpdatedAtUtc = gist.UpdatedAt;
                    }

                    _semaphoreSlim.Release();
                });

                _timer.Interval = TimeSpan.FromSeconds(_appSettingService.Settings().GitHub.StatusRefreshIntervalSeconds).TotalMilliseconds;
                _timer.Start();
            }
        }

        public void AddWatch(GistWatch gistWatch)
        {
            lock (_watchesLock)
            {
                // Register file watch
                if (!_watches.ContainsKey(gistWatch.GistId))
                    _watches[gistWatch.GistId] = new List<GistWatch> { gistWatch };
                else
                    _watches[gistWatch.GistId].Add(gistWatch);
            }
        }

        public void Dispose()
        {
            _timer?.Stop();
            _timer?.Dispose();
        }
    }
}
