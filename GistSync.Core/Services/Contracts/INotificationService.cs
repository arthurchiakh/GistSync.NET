namespace GistSync.Core.Services.Contracts
{
    public interface INotificationService
    {
        void NotifyFileUpdated(string filePath);
        void NotifyGistUpdated(string gistId);
    }
}
