using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GistSync.Core.Models
{
    [Table("SyncTasks")]
    public class SyncTask
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string GistId { get; set; }
        public SyncModeTypes SyncMode { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Directory { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? GitHubPersonalAccessToken { get; set; }
        public bool IsEnabled { get; set; }
        public ICollection<SyncTaskFile> Files { get; set; }
    }
 }