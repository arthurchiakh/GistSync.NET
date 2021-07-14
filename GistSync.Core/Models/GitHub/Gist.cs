using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GistSync.Core.Models.GitHub
{
    public class Gist
    {
        public string Url { get; set; }
        [JsonPropertyName("forks_url")]
        public string ForksUrl { get; set; }
        [JsonPropertyName("commits_url")]
        public string CommitsUrl { get; set; }
        public string Id { get; set; }
        [JsonPropertyName("node_id")]
        public string NodeId { get; set; }
        [JsonPropertyName("git_pull_url")]
        public string GitPullUrl { get; set; }
        [JsonPropertyName("git_push_url")]
        public string GitPushUrl { get; set; }
        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; }
        public Dictionary<string, File> Files { get; set; }
        public bool Public { get; set; }
        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }
        public string Description { get; set; }
        public int Comments { get; set; }
        public string User { get; set; }
        [JsonPropertyName("comments_url")]
        public string CommentsUrl { get; set; }
        public Owner Owner { get; set; }
        public bool Truncated { get; set; }
    }
}
