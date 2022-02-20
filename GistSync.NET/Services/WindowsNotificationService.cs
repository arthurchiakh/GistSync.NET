using GistSync.Core.Services.Contracts;
using Microsoft.Toolkit.Uwp.Notifications;

namespace GistSync.NET.Services
{
    public class WindowsNotificationService : INotificationService
    {
        public void NotifyFileUpdated(string filePath)
        {
            new ToastContentBuilder()
                //.AddArgument("action", "viewConversation")
                //.AddArgument("conversationId", 9813)
                .AddText("File updated.")
                .AddText(filePath)
                .Show();
        }

        public void NotifyGistUpdated(string gistId)
        {
            new ToastContentBuilder()
                //.AddArgument("action", "viewConversation")
                //.AddArgument("conversationId", 9813)
                .AddText("Gist updated.")
                .AddText(gistId)
                .Show();
        }
    }
}
