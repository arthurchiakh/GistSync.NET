using System;

namespace GistSync.Core.Models
{
    public sealed class GistWatch
    {
        public int SyncTaskId { get; set; }
        public string GistId { get; set; }
        public string PersonalAccessToken { get; set; }
        public GitHub.File[] Files { get; set; }
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
        public GitHub.File[] Files { get; }
        public GistUpdatedEventArgs(string gistId, DateTime updatedAtUtc, GitHub.File[] files)
        {
            GistId = gistId;
            UpdatedAtUtc = updatedAtUtc;
            Files = files;
        }
    }
}