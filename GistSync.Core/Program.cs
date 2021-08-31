using System.Threading.Tasks;

namespace GistSync.Core
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            await new GistSyncHost().Start();
        }
    }
}
