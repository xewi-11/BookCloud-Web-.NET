using BookCloud.Helpers;
using BookCloud.Models.ViewModels;
using BookCloud.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookCloud.Controllers
{
    public class CarritoController : Controller
    {
        private readonly IRepositoryLibros _repoLibros;

        public CarritoController(IRepositoryLibros repoLibros)
        {
            _repoLibros = repoLibros;
        }

        // Vista principal del carrito
        public IActionResult Index()
        {
            var userIdStr = HttpContext.Session.GetString("Id");
            if (string.IsNullOrEmpty(userIdStr))
                return RedirectToAction("Login", "Auth");

            var items = CarritoHelper.GetCarrito(HttpContext.Session);
            var viewModel = new CarritoViewModel { Items = items };

            return View(viewModel);
        }

        // Agregar libro al carrito
        [HttpPost]
        public async Task<IActionResult> AgregarAlCarrito(int libroId, int cantidad = 1)
        {
            var userIdStr = HttpContext.Session.GetString("Id");
            if (string.IsNullOrEmpty(userIdStr))
                return RedirectToAction("Login", "Auth");

            try
            {
                // Obtener información del libro
                var libro = await _repoLibros.GetLibro(libroId);

                if (libro == null)
                {
                    TempData["Error"] = "El libro no existe o no está disponible.";
                    return RedirectToAction("Index", "Libro");
                }

                if (libro.Stock < cantidad)
                {
                    TempData["Error"] = $"Stock insuficiente. Solo hay {libro.Stock} unidades disponibles.";
                    return RedirectToAction("Details", "Libro", new { id = libroId });
                }

                // Crear item del carrito
                var carritoItem = new CarritoItem
                {
                    LibroId = libro.Id,
                    Titulo = libro.Titulo,
                    Autor = libro.Autor,
                    Foto = libro.Foto,
                    Precio = libro.Precio,
                    Cantidad = cantidad,
                    StockDisponible = libro.Stock
                };

                // Agregar a session
                CarritoHelper.AgregarItem(HttpContext.Session, carritoItem);

                TempData["Mensaje"] = $"'{libro.Titulo}' agregado al carrito.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al agregar al carrito: {ex.Message}";
                return RedirectToAction("Index", "Libro");
            }
        }

        // Actualizar cantidad de un item
        [HttpPost]
        public IActionResult ActualizarCantidad(int libroId, int cantidad)
        {
            var userIdStr = HttpContext.Session.GetString("Id");
            if (string.IsNullOrEmpty(userIdStr))
                return RedirectToAction("Login", "Auth");

            try
            {
                CarritoHelper.ActualizarCantidad(HttpContext.Session, libroId, cantidad);
                TempData["Mensaje"] = "Cantidad actualizada.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        // Eliminar item del carrito
        [HttpPost]
        public IActionResult EliminarItem(int libroId)
        {
            var userIdStr = HttpContext.Session.GetString("Id");
            if (string.IsNullOrEmpty(userIdStr))
                return RedirectToAction("Login", "Auth");

            CarritoHelper.EliminarItem(HttpContext.Session, libroId);
            TempData["Mensaje"] = "Producto eliminado del carrito.";

            return RedirectToAction("Index");
        }

        // Limpiar todo el carrito
        [HttpPost]
        public IActionResult LimpiarCarrito()
        {
            var userIdStr = HttpContext.Session.GetString("Id");
            if (string.IsNullOrEmpty(userIdStr))
                return RedirectToAction("Login", "Auth");

            CarritoHelper.LimpiarCarrito(HttpContext.Session);
            TempData["Mensaje"] = "Carrito vaciado.";

            return RedirectToAction("Index");
        }

        // API para obtener cantidad de items (para badge en navbar)
        [HttpGet]
        public IActionResult GetCantidadItems()
        {
            var cantidad = CarritoHelper.GetCantidadTotal(HttpContext.Session);
            return Json(new { cantidad });
        }
    }
}