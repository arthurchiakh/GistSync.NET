using System.Text.Json.Serialization;

namespace GistSync.Core.Models.GitHub
{
    public class File
    {
        public string FileName { get; set; }
        public string Type { get; set; }
        public string Language { get; set; }
        [JsonPropertyName("raw_url")]
        public string RawUrl { get; set; }
        public long Size { get; set; }
    }
}
