using BookCloud.Helpers;
using BookCloud.Models;
using BookCloud.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static BookCloud.Helpers.FolderHelper;

namespace BookCloud.Controllers
{
    public class LibroController : Controller
    {
        private IRepositoryLibros repo;
        private FotoLibro _fotoHelper;
        public LibroController(IRepositoryLibros repo, FotoLibro fotoHelper)
        {
            this.repo = repo;
            this._fotoHelper = fotoHelper;
        }
        public async Task<IActionResult> Index()
        {
            List<Libro> libros = await repo.GetLibros();
            return View(libros);
        }
        public async Task<IActionResult> Details(int id)
        {
            Libro libro = await repo.GetLibro(id);

            if (libro == null)
            {
                TempData["Error"] = "El libro solicitado no existe o no está disponible.";
                return RedirectToAction("Index");
            }

            return View(libro);
        }
        public IActionResult Create()
        {
            // Verificar si hay sesión activa
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Id")))
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(string Titulo, string Autor, string Descripcion, IFormFile imagenFile, decimal Precio, int Stock, DateTime FechaPublicacion, bool Activo)
        {
            var UsuarioId = int.Parse(HttpContext.Session.GetString("Id"));
            string nameFoto = $"{UsuarioId}_{Titulo}_{imagenFile.FileName}";
            Libro libro = new Libro();
            if (imagenFile != null && imagenFile.Length > 0 && UsuarioId != null)
            {
                string path = this._fotoHelper.MapPath(imagenFile.FileName, Folder.Libros, UsuarioId, Titulo);
                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    await imagenFile.CopyToAsync(stream);
                }
                string pathweb = this._fotoHelper.MapUrlPath(imagenFile.FileName, Folder.Libros, UsuarioId);
                ViewData["PATHWEB"] = pathweb;
                libro.Titulo = Titulo;
                libro.Autor = Autor;
                libro.Descripcion = Descripcion;
                libro.Foto = nameFoto;
                libro.Precio = Precio;
                libro.Stock = Stock;
                libro.UsuarioId = UsuarioId;
                libro.FechaPublicacion = FechaPublicacion;
                libro.Activo = Activo;
            }

            int id = await repo.InsertLibro(libro);
            return RedirectToAction("Details", new { id = id });
        }

        // GET: Libro/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            // Verificar si hay sesión activa
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Id")))
            {
                return RedirectToAction("Login", "Auth");
            }

            Libro libro = await repo.GetLibro(id);

            if (libro == null)
            {
                TempData["Error"] = "El libro no existe o no está disponible.";
                return RedirectToAction("Index");
            }

            // Verificar que el usuario sea el propietario del libro
            var usuarioId = int.Parse(HttpContext.Session.GetString("Id"));
            if (libro.UsuarioId != usuarioId)
            {
                TempData["Error"] = "No tienes permiso para editar este libro.";
                return RedirectToAction("Details", new { id = libro.Id });
            }

            return View(libro);
        }

        // POST: Libro/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string Titulo, string Autor, string Descripcion, IFormFile imagenFile, decimal Precio, int Stock, DateTime FechaPublicacion)
        {
            // Verificar sesión
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Id")))
            {
                return RedirectToAction("Login", "Auth");
            }

            Libro libroExistente = await repo.GetLibro(id);

            if (libroExistente == null)
            {
                TempData["Error"] = "El libro no existe.";
                return RedirectToAction("Index");
            }

            // Verificar propiedad
            var usuarioId = int.Parse(HttpContext.Session.GetString("Id"));
            if (libroExistente.UsuarioId != usuarioId)
            {
                TempData["Error"] = "No tienes permiso para editar este libro.";
                return RedirectToAction("Details", new { id = id });
            }

            // Actualizar datos
            libroExistente.Titulo = Titulo;
            libroExistente.Autor = Autor;
            libroExistente.Descripcion = Descripcion;
            libroExistente.Precio = Precio;
            libroExistente.Stock = Stock;
            libroExistente.FechaPublicacion = FechaPublicacion;

            // Si se sube nueva imagen
            if (imagenFile != null && imagenFile.Length > 0)
            {
                string nameFoto = $"{usuarioId}_{Titulo}_{imagenFile.FileName}";
                string path = this._fotoHelper.MapPath(imagenFile.FileName, Folder.Libros, usuarioId, Titulo);

                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    await imagenFile.CopyToAsync(stream);
                }

                libroExistente.Foto = nameFoto;
            }

            await repo.UpdateLibro(libroExistente);

            TempData["Success"] = "Libro actualizado correctamente.";
            return RedirectToAction("Details", new { id = id });
        }

        // GET: Libro/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            // Verificar si hay sesión activa
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Id")))
            {
                return RedirectToAction("Login", "Auth");
            }

            Libro libro = await repo.GetLibro(id);

            if (libro == null)
            {
                TempData["Error"] = "El libro no existe o no está disponible.";
                return RedirectToAction("Index");
            }

            // Verificar que el usuario sea el propietario del libro
            var usuarioId = int.Parse(HttpContext.Session.GetString("Id"));
            if (libro.UsuarioId != usuarioId)
            {
                TempData["Error"] = "No tienes permiso para eliminar este libro.";
                return RedirectToAction("Details", new { id = libro.Id });
            }

            return View(libro);
        }

        // POST: Libro/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Verificar sesión
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Id")))
            {
                return RedirectToAction("Login", "Auth");
            }

            Libro libro = await repo.GetLibro(id);

            if (libro == null)
            {
                TempData["Error"] = "El libro no existe.";
                return RedirectToAction("Index");
            }

            // Verificar propiedad
            var usuarioId = int.Parse(HttpContext.Session.GetString("Id"));
            if (libro.UsuarioId != usuarioId)
            {
                TempData["Error"] = "No tienes permiso para eliminar este libro.";
                return RedirectToAction("Index");
            }

            await repo.DeleteLibro(id);

            TempData["Success"] = "Libro eliminado correctamente.";
            return RedirectToAction("Index");
        }
    }
}