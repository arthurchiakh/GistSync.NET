using System;
using System.Text.Json.Serialization;

namespace GistSync.Core.Models
{
    public class SyncTask
    {
        public string GistId { get; set; }
        public SyncStrategyTypes SyncStrategyType { get; set; }
        public DateTime? GistUpdatedAt { get; set; }
        public string GistFileName { get; set; }
        public string MappedLocalFilePath { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string FileChecksum { get; set; }
        public string GitHubPersonalAccessToken { get; set; }
    }
}