namespace BookCloud.Models.ViewModels
{
    public class PedidoViewModel
    {
        public int PedidoId { get; set; }
        public DateTime FechaPedido { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
        public List<CarritoItem> Items { get; set; }
        public MetodoPagoViewModel MetodoPago { get; set; }
    }


}