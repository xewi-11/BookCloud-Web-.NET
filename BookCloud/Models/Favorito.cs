using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookCloud.Models
{
    [Table("Favoritos")]
    public class Favorito
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [Column("UsuarioId")]
        public int UsuarioId { get; set; }

        [Required]
        [Column("LibroId")]
        public int LibroId { get; set; }

        [Column("FechaAgregado")]
        public DateTime FechaAgregado { get; set; }

        [Required]
        [Column("Activo")]
        public bool Activo { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }

        [ForeignKey("LibroId")]
        public Libro Libro { get; set; }
    }
}
