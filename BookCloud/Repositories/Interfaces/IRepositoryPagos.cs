using BookCloud.Models;

namespace BookCloud.Repositories.Interfaces
{
    public interface IRepositoryPagos
    {
        Task<int> CrearPago(Pago pago);
        Task<Pago> GetPago(int pagoId);
        Task<List<Pago>> GetPagosPorPedido(int pedidoId);
        Task<List<Pago>> GetPagosPorUsuario(int usuarioId);
    }
}