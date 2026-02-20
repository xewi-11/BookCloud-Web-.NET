using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookCloud.Models
{
    [Table("CarritoItems")]
    public class CarritoItem
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [Column("CarritoId")]
        public int CarritoId { get; set; }

        [Required]
        [Column("LibroId")]
        public int LibroId { get; set; }

        [Required]
        [Column("Cantidad")]
        public int Cantidad { get; set; }

        [Required]
        [Column("PrecioUnitario")]
        public decimal PrecioUnitario { get; set; }

        [Required]
        [Column("Activo")]
        public bool Activo { get; set; }

        [ForeignKey("CarritoId")]
        public Carrito Carrito { get; set; }

        [ForeignKey("LibroId")]
        public Libro Libro { get; set; }
    }
}
