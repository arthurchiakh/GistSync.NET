using System.Threading.Tasks;
using GistSync.Core;
using GistSync.Core.Services.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GistSync.Windows
{
    class Program
    {
        private static IHost _host;

        private static async Task Main(string[] args)
        {
            var builder = new GistSyncHost().GetDefaultHostBuilder(args);

            builder.ConfigureServices(c =>
            {
                // Override with Windows specified services
                c.AddSingleton<IAppDataService, WindowsAppDataService>();
                c.AddSingleton<INotificationService, WindowsNotificationService>();
            });

            _host = builder.Build();

            using (_host)
                await _host.RunAsync();
        }
    }
}