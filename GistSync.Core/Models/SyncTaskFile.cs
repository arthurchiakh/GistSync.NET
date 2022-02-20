using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GistSync.Core.Models
{
    [Table("SyncTaskFiles")]
    public class SyncTaskFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SyncTaskId { get; set; }
        public string FileName { get; set; }
        public string? FileChecksum { get; set; }
    }
}
