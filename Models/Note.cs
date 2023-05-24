using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomaWeb.Models {
    [Table("Notes")]
    public class Note {
        [Column("id")]
        [Key]
        public int? Id { get; set; }

        [Column("UserName")]
        public string? UserName { get; set; }

        [Column("Name")]
        public string? Name { get; set; }

        [Column("BranchName")]
        public string? BranchName { get; set; }

        [Column("Version")]
        public int? Version { get; set; }

        [Column("Text")]
        public string? Text { get; set; }

        [Column("CreatingDate")]
        public DateTime? CreatingDate { get; set; }
        
    }
}
