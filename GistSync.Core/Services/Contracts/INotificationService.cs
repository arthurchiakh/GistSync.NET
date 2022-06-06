namespace GistSync.Core.Services.Contracts
{
    public interface INotificationService
    {
        /// <summary>
        /// File has been updated due to newer content detected from Gist online.
        /// </summary>
        /// <param name="filePath">Local file path</param>
        void NotifyFileUpdated(string filePath);
        /// <summary>
        /// Gist content has been updated to local file content change detected.
        /// </summary>
        /// <param name="gistId">Gist Id</param>
        void NotifyGistUpdated(string gistId);
    }
}
