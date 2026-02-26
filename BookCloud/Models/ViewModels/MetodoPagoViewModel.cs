namespace BookCloud.Models.ViewModels
{
    public class MetodoPagoViewModel
    {
        public decimal Total { get; set; }
        public decimal SaldoDisponible { get; set; }
        public bool TieneSaldoSuficiente { get; set; }
    }
}