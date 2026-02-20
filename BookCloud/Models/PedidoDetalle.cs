using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookCloud.Models
{
    [Table("PedidoDetalle")]
    public class PedidoDetalle
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [Column("PedidoId")]
        public int PedidoId { get; set; }

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

        [ForeignKey("PedidoId")]
        public Pedido Pedido { get; set; }

        [ForeignKey("LibroId")]
        public Libro Libro { get; set; }
    }
}
