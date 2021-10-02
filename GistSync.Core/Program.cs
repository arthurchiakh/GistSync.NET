using System.Threading.Tasks;

namespace GistSync.Core
{
    public class Program
    {
        static async Task Main()
        {
            await new GistSyncHost().Start();
        }
    }
}
