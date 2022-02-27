using GistSync.Core;
using GistSync.Core.Services.Contracts;
using GistSync.NET.Services;
using GistSync.NET.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WindowsFormsLifetime;

namespace GistSync.NET
{
    internal static class Program
    {
        private static IHost _host;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        async static Task Main()
        {
            var hostBuilder = GistSyncCoreHost.CreateDefaultHostBuilder();
            hostBuilder
                .UseWindowsFormsLifetime<MainForm>(options =>
            {
                options.HighDpiMode = HighDpiMode.SystemAware;
                options.EnableVisualStyles = true;
                options.CompatibleTextRenderingDefault = false;
                options.SuppressStatusMessages = false;
                options.EnableConsoleShutdown = true;
            })
                .ConfigureServices(services =>
            {
                services.AddSingleton<INotificationService, WindowsNotificationService>();
                services.AddForm<MainForm>();
                services.AddForm<NewTaskForm>();
            });

            // Build host
            _host = hostBuilder.Build();
            await _host.Services.GetRequiredService<GistSyncDbContext>().Database.MigrateAsync()!;
            await _host.RunAsync();
        }
    }
}