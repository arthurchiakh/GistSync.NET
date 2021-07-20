using GistSync.Core.Services.Contracts;

namespace GistSync.Core.Services
{
    public class DefaultNotificationService : INotificationService
    {
        public void NotifyFileUpdated(string filePath)
        {
        }

        public void NotifyGistUpdated(string gistId)
        {
        }
    }
}