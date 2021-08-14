using System.IO;
using GistSync.Core.Services.Contracts;
using Microsoft.Toolkit.Uwp.Notifications;

namespace GistSync.Windows.Services
{
    public class WindowsNotificationService : INotificationService
    {
        public void NotifyFileUpdated(string filePath)
        {
            new ToastContentBuilder()
                .AddText($"File {Path.GetFileName(filePath)} has been updated.")
                .AddText("Newer Gist revision detected.")
                .Show();
        }

        public void NotifyGistUpdated(string gistId)
        {
            new ToastContentBuilder()
                .AddText($"Gist {gistId} updated")
                .AddText("Local file change detected.")
                .Show();
        }
    }
}
