using BookCloud.Data;
using BookCloud.Models;
using BookCloud.Models.ViewModels;
using BookCloud.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookCloud.Controllers
{
    public class WalletController : Controller
    {
        private readonly IRepositoryWallet _repoWallet;
        private readonly BookCloudContext _context;

        public WalletController(IRepositoryWallet repoWallet, BookCloudContext context)
        {
            _repoWallet = repoWallet;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userIdStr = HttpContext.Session.GetString("Id");
            if (string.IsNullOrEmpty(userIdStr))
                return RedirectToAction("Login", "Auth");

            int usuarioId = int.Parse(userIdStr);

            var viewModel = new WalletViewModel
            {
                SaldoActual = await _repoWallet.GetSaldoUsuario(usuarioId),
                Movimientos = await _repoWallet.GetMovimientos(usuarioId, 20)
            };

            // Calcular totales
            viewModel.TotalIngresos = viewModel.Movimientos
                .Where(m => m.Tipo == "Ingreso")
                .Sum(m => m.Monto);

            viewModel.TotalGastos = viewModel.Movimientos
                .Where(m => m.Tipo == "Pago" || m.Tipo == "Retiro")
                .Sum(m => m.Monto);

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Movimientos(int limite = 50, string filtro = "Todos")
        {
            var userIdStr = HttpContext.Session.GetString("Id");
            if (string.IsNullOrEmpty(userIdStr))
                return RedirectToAction("Login", "Auth");

            int usuarioId = int.Parse(userIdStr);

            // Obtener movimientos de Wallet
            var movimientosWallet = await _context.SaldoMovimientos
                .Where(m => m.UsuarioId == usuarioId && m.Activo)
                .Select(m => new MovimientoUnificadoViewModel
                {
                    Fecha = m.Fecha,
                    Tipo = m.Tipo,
                    Metodo = "Wallet",
                    Monto = m.Monto,
                    Descripcion = m.Descripcion,
                    PedidoId = m.PedidoId
                })
                .ToListAsync();

            // Obtener pagos con tarjeta
            var pagosTarjeta = await _context.Pagos
                .Include(p => p.Pedido)
                .Where(p => p.Pedido.UsuarioId == usuarioId && p.Activo && p.Metodo == "Tarjeta")
                .Select(p => new MovimientoUnificadoViewModel
                {
                    Fecha = p.FechaPago,
                    Tipo = "Pago",
                    Metodo = "Tarjeta",
                    Monto = p.Monto,
                    Descripcion = $"Compra con tarjeta - Pedido #{p.PedidoId}",
                    PedidoId = p.PedidoId
                })
                .ToListAsync();

            // Combinar todos los movimientos
            var todosMovimientos = movimientosWallet
                .Concat(pagosTarjeta)
                .OrderByDescending(m => m.Fecha)
                .ToList();

            // Aplicar filtro según parámetro
            var movimientosFiltrados = filtro switch
            {
                "Wallet" => todosMovimientos.Where(m => m.Metodo == "Wallet").Take(limite).ToList(),
                "Tarjeta" => todosMovimientos.Where(m => m.Metodo == "Tarjeta").Take(limite).ToList(),
                _ => todosMovimientos.Take(limite).ToList() // "Todos"
            };

            var viewModel = new MovimientosViewModel
            {
                SaldoActual = await _repoWallet.GetSaldoUsuario(usuarioId),
                Movimientos = movimientosFiltrados
            };

            // Calcular totales sobre todos los movimientos (no filtrados)
            viewModel.TotalIngresos = todosMovimientos
                .Where(m => m.Tipo == "Ingreso")
                .Sum(m => m.Monto);

            viewModel.TotalGastos = todosMovimientos
                .Where(m => m.Tipo == "Pago" || m.Tipo == "Retiro")
                .Sum(m => m.Monto);

            viewModel.TotalGastosWallet = todosMovimientos
                .Where(m => (m.Tipo == "Pago" || m.Tipo == "Retiro") && m.Metodo == "Wallet")
                .Sum(m => m.Monto);

            viewModel.TotalGastosTarjeta = todosMovimientos
                .Where(m => m.Tipo == "Pago" && m.Metodo == "Tarjeta")
                .Sum(m => m.Monto);

            ViewBag.FiltroActual = filtro;

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> RecargarSaldo(decimal monto)
        {
            var userIdStr = HttpContext.Session.GetString("Id");
            if (string.IsNullOrEmpty(userIdStr))
                return RedirectToAction("Login", "Auth");

            int usuarioId = int.Parse(userIdStr);

            try
            {
                if (monto < 1 || monto > 10000)
                {
                    TempData["Error"] = "El monto debe estar entre $1 y $10,000";
                    return RedirectToAction("Index");
                }

                await _repoWallet.RecargarSaldo(usuarioId, monto, $"Recarga de ${monto:N2}");
                TempData["Mensaje"] = $"¡Recarga exitosa! Se agregaron ${monto:N2} a tu wallet.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al recargar: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}