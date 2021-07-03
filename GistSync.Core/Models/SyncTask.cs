using System;

namespace GistSync.Core.Models
{
    public class SyncTask
    {
        public string Guid { get; set; }
        public SyncStrategyTypes SyncStrategyType { get; set; }
        public string GistId { get; set; }
        public DateTime GistUpdatedAt { get; set; }
        public string GistFileName { get; set; }
        public string LocalFilePath { get; set; }
        public string AccessTokenId { get; set; } // TODO: to get saved personal access token later
    }
}