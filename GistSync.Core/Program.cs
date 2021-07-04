using System.Threading.Tasks;
using GistSync.Core.Factories;
using GistSync.Core.Factories.Contracts;
using GistSync.Core.Services;
using GistSync.Core.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GistSync.Core
{
    public static class Program
    {
        private static IHost _host;

        public static async Task Main(string[] args)
        {
            var builder = new HostBuilder();
            builder.ConfigureHostConfiguration(configHost =>
            {
                configHost.AddEnvironmentVariables(prefix: "ASPNETCORE_");
                configHost.AddCommandLine(args);
            })
            .ConfigureAppConfiguration((context, config) =>
            {
                var env = context.HostingEnvironment;

                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
            })
            .ConfigureServices((context, c) =>
            {
                var env = context.HostingEnvironment;

                c.AddSingleton<IFileChecksumService, Md5FileChecksumService>();
                c.AddSingleton<IGitHubApiService, GitHubApiService>();
                c.AddSingleton<IAppDataService, WindowsAppDataService>();
                c.AddSingleton<IAppSettingService, JsonAppSettingService>();
                c.AddSingleton<IGistWatchFactory, GistWatchFactory>();
                c.AddSingleton<IGistWatcherService, GistWatcherService>();
                c.AddSingleton<IFileWatchFactory, FileWatchFactory>();
                c.AddSingleton<IFileWatcherService, FileWatcherService>();

                c.AddHostedService<GistSyncBackgroundService>();
            })
            .ConfigureLogging((context, b) =>
            {
                b.AddConsole();
            });

            _host = builder.Build();

            using (_host)
                await _host.RunAsync();
        }
    }
}