namespace BookCloud.Models
{
    public class WalletViewModel
    {
        public decimal SaldoActual { get; set; }
        public List<SaldoMovimiento> Movimientos { get; set; } = new();
        public decimal TotalIngresos { get; set; }
        public decimal TotalGastos { get; set; }
    }
}
