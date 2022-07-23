using GistSync.Core.Services.Contracts;
using Microsoft.Toolkit.Uwp.Notifications;

namespace GistSync.NET.Services
{
    public class WindowsNotificationService : INotificationService
    {
        public void NotifyFileUpdated(string filePath)
        {
            new ToastContentBuilder()
                .AddText($"{Path.GetFileName(filePath)} has been updated.")
                .SetProtocolActivation(new Uri($"file://{filePath}"))
                .Show();
        }

        public void NotifyFileAdded(string filePath)
        {
            new ToastContentBuilder()
                .AddText($"{Path.GetFileName(filePath)} has been added.")
                .SetProtocolActivation(new Uri($"file://{filePath}"))
                .Show();
        }

        public void NotifyGistUpdated(string gistId)
        {
            new ToastContentBuilder()
                .AddText($"Gist {gistId} has been updated due to local file changed.")
                .SetProtocolActivation(new Uri($"https://gist.github.com/{gistId}"))
                .Show();
        }
    }
}
