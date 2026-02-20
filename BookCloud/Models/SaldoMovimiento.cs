using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookCloud.Models
{
    public class SaldoMovimiento
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("UsuarioId")]
        public int UsuarioId { get; set; }

        [Column("PedidoId")]
        public int? PedidoId { get; set; }

        [Column("Monto")]
        public decimal Monto { get; set; }

        [Column("Tipo")]
        [MaxLength(50)]
        public string Tipo { get; set; }

        [Column("Descripcion")]
        [MaxLength(255)]
        public string? Descripcion { get; set; }

        [Column("Fecha")]
        public DateTime Fecha { get; set; }

        [Column("Activo")]
        public bool Activo { get; set; }

        // 🔹 Navigation Properties

        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }

        [ForeignKey("PedidoId")]
        public Pedido? Pedido { get; set; }
    }
}
