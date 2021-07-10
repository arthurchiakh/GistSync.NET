using System.Threading.Tasks;

namespace GistSync.Core.ConsoleRunner
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = new GistSyncHost();
            await host.Start(args);
        }
    }
}
