/*
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using GistSync.Core.Models.GitHub;
using GistSync.Core.Services.Contracts;
using Microsoft.Extensions.Options;
using Timer = System.Timers.Timer;



namespace GistSync.Core.Services
{
    public record UpdatedGist(string GistId, DateTime UpdatedAtUtc);
    public record GistMonitorSettings(int MaxConcurrentGistApiCalls = 2, int StatusRefreshIntervalSeconds = 300);

    public class GistMonitor
    {
        private readonly IList<GistFileUpdater> _observers;
        private readonly SemaphoreSlim _semaphoreSlim;
        private readonly IOptions<GistMonitorSettings> _options;
        private readonly IGitHubApiService _gitHubApiService;
        private readonly Timer _timer;

        public GistMonitor(IOptions<GistMonitorSettings> options, IGitHubApiService gitHubApiService)
        {
            _options = options;
            _gitHubApiService = gitHubApiService;
            _observers = new List<GistFileUpdater>();
            _semaphoreSlim = new SemaphoreSlim(_options.Value.MaxConcurrentGistApiCalls);

            _timer = new Timer
            {
                Enabled = true,
                AutoReset = false
            };
            _timer.Elapsed += (sender, args) =>
            {
                _observers.Select(o => o.GistId).Distinct().AsParallel().ForAll(async w =>
                {
                    await _semaphoreSlim.WaitAsync();

                    try
                    {
                        var gist = await _gitHubApiService.Gist(w);

                        foreach (var o in _observers.Where(o => o.GistId == w))
                            o.OnNext(new UpdatedGist(w, gist.UpdatedAt!.Value));
                    }
                    finally
                    {
                        _semaphoreSlim.Release();
                    }
                });

                _timer.Interval = TimeSpan.FromSeconds(_options.Value.StatusRefreshIntervalSeconds).TotalMilliseconds;
                _timer.Start();
            };
            _timer.Start();

        }

        public IDisposable Subscribe(GistFileUpdater observer)
        {
            if (!_observers.Contains(observer))
                _observers.Remove(observer);

            return new Unsubscriber(_observers, observer);
        }
        public class Unsubscriber : IDisposable
        {
            private readonly IList<GistFileUpdater> _observers;
            private readonly GistFileUpdater _observer;

            public Unsubscriber(IList<GistFileUpdater> observers, GistFileUpdater observer)
            {
                _observers = observers;
                _observer = observer;
            }

            void IDisposable.Dispose()
            {
                if (_observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }
    }

    public class GistFileUpdater : IObserver<UpdatedGist>
    {
        private string _gistId;

        public GistFileUpdater(string gistId)
        {
            _gistId = gistId;
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(UpdatedGist value)
        {
        }
    }
}



*/