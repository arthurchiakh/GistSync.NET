using System;

namespace GistSync.Core.Models
{
    public sealed class FileWatch
    {
        internal FileWatch() { }

        private string _fileHash;
        public string FilePath { get; set; }
        public DateTime? ModifiedDateTimeUtc { get; set; }
        public string FileHash
        {
            get => _fileHash;
            set
            {
                // Do not trigger event for first update
                if (_fileHash == null)
                {
                    _fileHash = value;
                    return;
                }

                // Do nothing when the value remain unchanged
                if (_fileHash.Equals(value, StringComparison.OrdinalIgnoreCase)) return;


                _fileHash = value;
                FileContentChangedEvent?.Invoke(this);
            }
        }

        public event FileContentChangedEventHandler FileContentChangedEvent;
    }

    public delegate void FileContentChangedEventHandler(FileWatch fileWatch);
}