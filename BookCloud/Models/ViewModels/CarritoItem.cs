namespace BookCloud.Models.ViewModels
{
    public class CarritoItem
    {
        public int LibroId { get; set; }
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public string? Foto { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public int StockDisponible { get; set; }

        // Calculado
        public decimal Subtotal => Precio * Cantidad;
    }
}