using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomaWeb.Models {
    [Table("Users")]
    public class User {
        [Key]
        [Column("Login")]
        public string Login { get; set; } = null!;
        [Column("Password")]
        public string Password { get; set; } = null!;

    }
}
