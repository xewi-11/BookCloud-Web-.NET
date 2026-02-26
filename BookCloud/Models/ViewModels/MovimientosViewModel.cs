namespace BookCloud.Models.ViewModels
{
    public class MovimientosViewModel
    {
        public decimal SaldoActual { get; set; }
        public List<MovimientoUnificadoViewModel> Movimientos { get; set; } = new();
        public decimal TotalIngresos { get; set; }
        public decimal TotalGastos { get; set; }
        public decimal TotalGastosWallet { get; set; }
        public decimal TotalGastosTarjeta { get; set; }
    }
}