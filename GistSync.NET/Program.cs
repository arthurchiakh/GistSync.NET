using GistSync.Core;
using GistSync.Core.Services.Contracts;
using GistSync.NET.Services;
using GistSync.NET.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
        async static Task Main(string[] args)
        {
            var hostBuilder = GistSyncCoreHost.CreateDefaultHostBuilder();
            hostBuilder.UseWindowsFormsLifetime<MainForm>(options =>
            {
                options.HighDpiMode = HighDpiMode.SystemAware;
                options.EnableVisualStyles = true;
                options.CompatibleTextRenderingDefault = false;
                options.SuppressStatusMessages = false;
                options.EnableConsoleShutdown = true;
            }).ConfigureServices(services =>
            {
                // Forms
                services.AddForm<NewTaskForm>();
                services.AddForm<SettingsForm>();
                services.AddForm<AboutForm>();

                // Options 
                services.Configure<MainFormOptions>(options =>
                    options.LaunchOnStartup = args.Length >= 1 && args[0] == "--startup");

                // Override notification
                services.AddSingleton<INotificationService, WindowsNotificationService>();
            }).ConfigureLogging(logBuilder =>
            {
                logBuilder.SetMinimumLevel(LogLevel.Information);
                logBuilder.AddFilter("Microsoft", LogLevel.Warning);
#if !DEBUG
                logBuilder.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
#endif
                logBuilder.AddActivityLogger();
            });

            // Build host
            _host = hostBuilder.Build();
            await _host.RunAsync();
        }
    }
}