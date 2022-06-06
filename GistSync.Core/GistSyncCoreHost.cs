using System.Diagnostics;
using System.Threading;
using GistSync.Core.Extensions;
using GistSync.Core.Factories;
using GistSync.Core.Factories.Contracts;
using GistSync.Core.Services;
using GistSync.Core.Services.Contracts;
using GistSync.Core.Strategies;
using GistSync.Core.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GistSync.Core;

public class GistSyncCoreHost
{
    private static CancellationTokenSource _cancellationTokenSource;

    public static IHostBuilder CreateDefaultHostBuilder()
    {
        _cancellationTokenSource = new CancellationTokenSource();

        var builder = Host.CreateDefaultBuilder();
        builder.ConfigureServices(services =>
        {
            services.AddDbContext<GistSyncDbContext>(ServiceLifetime.Transient);
            services.AddTransient<ISyncTaskDataService, SyncTaskDataService>();
            services.AddSingleton<IFileChecksumService, Md5FileChecksumService>();
            services.AddSingleton<IGitHubApiService, GitHubApiService>();
            services.AddSingleton<IGistWatchFactory, GistWatchFactory>();
            services.AddSingleton<IGistWatcherService, GistWatcherService>();
            services.AddSingleton<IFileWatchFactory, FileWatchFactory>();
            services.AddSingleton<IFileWatcherService, FileWatcherService>();
            services.AddSingleton<ISynchronizedFileAccessService, SynchronizedFileAccessService>();
            services.AddSingleton<IAppDataService, DefaultAppDataService>();
            services.AddSingleton<INotificationService, DefaultNotificationService>();
            services.RegisterSyncStrategyProvider();
            services.AddSingleton<GistSyncBackgroundService>();
            services.AddHostedService(sp => sp.GetRequiredService<GistSyncBackgroundService>());
        }).ConfigureAppConfiguration(configBuilder =>
        {
            configBuilder.Sources.Clear();
            configBuilder.Add(new DatabaseConfigurationSource(new GistSyncDbContext(new DefaultAppDataService(), new DbContextOptions<GistSyncDbContext>())));
        });

        return builder;
    }
}