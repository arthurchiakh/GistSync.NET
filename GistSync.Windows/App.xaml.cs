using System;
using System.Threading.Tasks;
using System.Windows;
using GistSync.Core;
using GistSync.Core.Services.Contracts;
using GistSync.Windows.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GistSync.Windows
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly GistSyncHost _host;
        public App()
        {
            _host = new GistSyncHost();
        }

        private async void App_OnStartup(object sender, StartupEventArgs e)
        {
            await Task.Run(async () =>
            {
                await _host.Start(Environment.GetCommandLineArgs(), builder =>
                {
                    // Override with Windows specified services
                    builder.Services.AddSingleton<IAppDataService, WindowsAppDataService>();
                    builder.Services.AddSingleton<INotificationService, WindowsNotificationService>();
                });
            });
        }

        private async void App_OnExit(object sender, ExitEventArgs e)
        {
            await _host.Stop();
        }
    }
}