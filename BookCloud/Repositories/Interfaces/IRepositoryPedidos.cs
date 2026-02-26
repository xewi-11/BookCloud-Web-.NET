using BookCloud.Models;

namespace BookCloud.Repositories.Interfaces
{
    public interface IRepositoryPedidos
    {
        Task<int> CrearPedido(int usuarioId, decimal total, List<PedidoDetalle> detalles);
        Task<Pedido> GetPedido(int pedidoId);
        Task<List<Pedido>> GetPedidosUsuario(int usuarioId);
        Task ActualizarEstadoPedido(int pedidoId, string estado);
    }
}