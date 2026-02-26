namespace BookCloud.Models.ViewModels
{
    public class CarritoViewModel
    {
        public List<CarritoItem> Items { get; set; } = new();
        public decimal Total => Items.Sum(i => i.Subtotal);
        public int TotalItems => Items.Sum(i => i.Cantidad);
        public bool TieneItems => Items.Any();
    }
}