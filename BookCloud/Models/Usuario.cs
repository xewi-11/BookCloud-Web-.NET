using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookCloud.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("Nombre")]
        public string Nombre { get; set; }

        [Column("Email")]
        public string Correo { get; set; }

        [Column("Password")]
        [StringLength(50)]
        public string? Password { get; set; }

        [Column("FechaRegistro")]
        public DateTime FechaRegistro { get; set; }

        [Column("Activo")]
        public bool Activo { get; set; }

        [Column("FotoUrl")]
        public string? Foto { get; set; }

        // Relación uno a uno con UsuarioSeguridad
        public UsuarioSeguridad? UsuarioSeguridad { get; set; }
    }
}
