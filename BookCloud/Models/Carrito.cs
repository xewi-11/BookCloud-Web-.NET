using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookCloud.Models
{
    [Table("Carritos")]
    public class Carrito
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [Column("UsuarioId")]
        public int UsuarioId { get; set; }

        [Column("FechaCreacion")]
        public DateTime FechaCreacion { get; set; }

        [Required]
        [Column("Activo")]
        public bool Activo { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }

        public ICollection<CarritoItem> CarritoItems { get; set; } = new List<CarritoItem>();
    }
}
