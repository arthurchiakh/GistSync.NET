using System;

namespace GistSync.Core.Models
{
    public sealed class GistWatch
    {
        private DateTime? _updatedAtUtc;

        public string GistId { get; set; }
        public string PersonalAccessToken { get; set; }
        public DateTime? UpdatedAtUtc
        {
            get => _updatedAtUtc;
            set
            {
                // Do do nothing if value remain unchanged
                if (value == default(DateTime) || // Handle default datetime assigned when constructed
                    value == _updatedAtUtc)
                    return;

                // Do not trigger event for first update
                if (!_updatedAtUtc.HasValue)
                {
                    _updatedAtUtc = value;
                    return;
                }

                _updatedAtUtc = value;
                GistUpdatedEvent?.Invoke(this, new GistUpdatedEventArgs(GistId, _updatedAtUtc.Value));
            }
        }

        public event GistUpdatedEventHandler GistUpdatedEvent;

        internal GistWatch() { }
    }

    public delegate void GistUpdatedEventHandler(object sender, GistUpdatedEventArgs args);

    public class GistUpdatedEventArgs : EventArgs
    {
        public string GistId { get; }
        public DateTime UpdatedAtUtc { get; }

        public GistUpdatedEventArgs(string gistId, DateTime updatedAtUtc)
        {
            GistId = gistId;
            UpdatedAtUtc = updatedAtUtc;
        }
    }
}