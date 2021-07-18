using System;

namespace GistSync.Core.Models
{
    public sealed class FileWatch
    {
        internal FileWatch() { }

        public string FilePath { get; set; }
        public DateTime? ModifiedDateTimeUtc { get; set; }
        public string Checksum { get; set; }

        public void TriggerFileContentChanged()
        {
            FileContentChangedEvent?.Invoke(this, new FileContentChangedEventArgs(FilePath, ModifiedDateTimeUtc.Value, Checksum));
        }

        public event FileContentChangedEventHandler FileContentChangedEvent;
    }

    public delegate void FileContentChangedEventHandler(object sender, FileContentChangedEventArgs args);

    public class FileContentChangedEventArgs : EventArgs
    {
        public string FilePath { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string Checksum { get; set; }
        public FileContentChangedEventArgs(string filePath, DateTime modifiedDateTime, string checksum)
        {
            FilePath = filePath;
            ModifiedDateTime = modifiedDateTime;
            Checksum = checksum;
        }
    }
}