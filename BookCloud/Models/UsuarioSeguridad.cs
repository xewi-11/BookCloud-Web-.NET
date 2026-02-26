using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookCloud.Models
{
    [Table("UsuarioCredenciales")]
    public class UsuarioSeguridad
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }
        [Column("UsuarioId")]
        public int UsuarioId { get; set; }
        [Column("PasswordHash")]
        public byte[] PasswordHash { get; set; }
        [Column("Salt")]
        public string Salt { get; set; }
        [Column("Activo")]
        public bool Activo { get; set; }

        // Navigation property
        public Usuario? Usuario { get; set; }

    }
}
