using System;
using GistSync.Core.Models.GitHub;

namespace GistSync.Core.Models
{
    public sealed class GistWatch
    {

        public string GistId { get; set; }
        public string PersonalAccessToken { get; set; }
        public File[] Files { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }

        public void TriggerGistUpdatedEvent()
        {
            GistUpdatedEvent?.Invoke(this, new GistUpdatedEventArgs(GistId, UpdatedAtUtc.Value, Files));
        }

        public event GistUpdatedEventHandler GistUpdatedEvent;

        internal GistWatch() { }
    }

    public delegate void GistUpdatedEventHandler(object sender, GistUpdatedEventArgs args);

    public class GistUpdatedEventArgs : EventArgs
    {
        public string GistId { get; }
        public DateTime UpdatedAtUtc { get; }
        public File[] Files { get; }
        public GistUpdatedEventArgs(string gistId, DateTime updatedAtUtc, File[] files)
        {
            GistId = gistId;
            UpdatedAtUtc = updatedAtUtc;
            Files = files;
        }
    }
}