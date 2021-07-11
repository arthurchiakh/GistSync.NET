using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GistSync.Core.Models.GitHub
{
    public class GistPatch
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("files")]
        public Dictionary<string, FilePatch> Files { get; set; }
    }
}
