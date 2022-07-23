using System.Text.Json.Serialization;

namespace GistSync.Core.Models.GitHub
{
    public class FilePatch
    {
        [JsonPropertyName("filename")]
        public string FileName { get; set; }
        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
} 