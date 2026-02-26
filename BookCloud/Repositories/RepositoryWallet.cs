using BookCloud.Data;
using BookCloud.Models;
using BookCloud.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookCloud.Repositories
{
    public class RepositoryWallet : IRepositoryWallet
    {
        private readonly BookCloudContext _context;

        public RepositoryWallet(BookCloudContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetSaldoUsuario(int usuarioId)
        {
            var saldo = await _context.SaldoMovimientos
                .Where(m => m.UsuarioId == usuarioId && m.Activo)
                .SumAsync(m => m.Tipo == "Ingreso" ? m.Monto : -m.Monto);

            return saldo;
        }

        public async Task<List<SaldoMovimiento>> GetMovimientos(int usuarioId, int limit = 20)
        {
            return await _context.SaldoMovimientos
                .Where(m => m.UsuarioId == usuarioId && m.Activo)
                .OrderByDescending(m => m.Fecha)
                .Take(limit)
                .Include(m => m.Pedido)
                .ToListAsync();
        }

        public async Task RecargarSaldo(int usuarioId, decimal monto, string descripcion)
        {
            if (monto <= 0)
                throw new ArgumentException("El monto debe ser mayor a 0");

            var movimiento = new SaldoMovimiento
            {
                UsuarioId = usuarioId,
                Monto = monto,
                Tipo = "Ingreso",
                Descripcion = descripcion ?? "Recarga de saldo",
                Fecha = DateTime.Now,
                Activo = true
            };

            await _context.SaldoMovimientos.AddAsync(movimiento);
            await _context.SaveChangesAsync();
        }

        public async Task DescontarSaldo(int usuarioId, int pedidoId, decimal monto, string descripcion)
        {
            if (monto <= 0)
                throw new ArgumentException("El monto debe ser mayor a 0");

            var saldoActual = await GetSaldoUsuario(usuarioId);
            if (saldoActual < monto)
                throw new InvalidOperationException("Saldo insuficiente");

            var movimiento = new SaldoMovimiento
            {
                UsuarioId = usuarioId,
                PedidoId = pedidoId,
                Monto = monto,
                Tipo = "Pago",
                Descripcion = descripcion ?? $"Pago de pedido #{pedidoId}",
                Fecha = DateTime.Now,
                Activo = true
            };

            await _context.SaldoMovimientos.AddAsync(movimiento);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> TieneSaldoSuficiente(int usuarioId, decimal monto)
        {
            var saldo = await GetSaldoUsuario(usuarioId);
            return saldo >= monto;
        }
    }
}