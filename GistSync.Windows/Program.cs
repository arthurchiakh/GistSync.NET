using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using GistSync.Core;
using GistSync.Core.Services.Contracts;
using GistSync.Windows.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GistSync.Windows
{
    class Program
    {
        [STAThread]
        static async Task Main(string[] args)
        {
            var builder = new GistSyncHost().GetDefaultHostBuilder(args);

            builder.ConfigureServices(c =>
            {
                // Override with Windows specified services
                c.AddSingleton<IAppDataService, WindowsAppDataService>();
                c.AddSingleton<INotificationService, WindowsNotificationService>();
            });

            var _host = builder.Build();

            using (_host)
                await _host.RunAsync();

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new App());
        }
    }
}