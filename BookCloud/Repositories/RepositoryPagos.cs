using BookCloud.Data;
using BookCloud.Models;
using BookCloud.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookCloud.Repositories
{
    public class RepositoryPagos : IRepositoryPagos
    {
        private readonly BookCloudContext _context;

        public RepositoryPagos(BookCloudContext context)
        {
            _context = context;
        }

        public async Task<int> CrearPago(Pago pago)
        {
            await _context.Pagos.AddAsync(pago);
            await _context.SaveChangesAsync();
            return pago.Id;
        }

        public async Task<Pago> GetPago(int pagoId)
        {
            return await _context.Pagos
                .Include(p => p.Pedido)
                .FirstOrDefaultAsync(p => p.Id == pagoId && p.Activo);
        }

        public async Task<List<Pago>> GetPagosPorPedido(int pedidoId)
        {
            return await _context.Pagos
                .Where(p => p.PedidoId == pedidoId && p.Activo)
                .OrderByDescending(p => p.FechaPago)
                .ToListAsync();
        }

        public async Task<List<Pago>> GetPagosPorUsuario(int usuarioId)
        {
            return await _context.Pagos
                .Include(p => p.Pedido)
                .Where(p => p.Pedido.UsuarioId == usuarioId && p.Activo)
                .OrderByDescending(p => p.FechaPago)
                .ToListAsync();
        }
    }
}