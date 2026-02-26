namespace BookCloud.Models.ViewModels
{
    public class MovimientoUnificadoViewModel
    {
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; } // "Ingreso", "Pago", "Retiro"
        public string Metodo { get; set; } // "Wallet", "Tarjeta"
        public decimal Monto { get; set; }
        public string Descripcion { get; set; }
        public int? PedidoId { get; set; }
    }
}