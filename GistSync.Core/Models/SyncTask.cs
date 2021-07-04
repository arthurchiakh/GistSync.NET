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
        public string MappedLocalFilePath { get; set; }
        public string GitHubPersonalAccessToken { get; set; }
    }
}