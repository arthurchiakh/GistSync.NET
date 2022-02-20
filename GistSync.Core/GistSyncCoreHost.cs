using GistSync.Core.Extensions;
using GistSync.Core.Factories;
using GistSync.Core.Factories.Contracts;
using GistSync.Core.Services;
using GistSync.Core.Services.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GistSync.Core;

public class GistSyncCoreHost
{
    public static IHostBuilder CreateDefaultHostBuilder()
    {
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
            services.AddHostedService<GistSyncBackgroundService>();
        });

        return builder;
    }
}