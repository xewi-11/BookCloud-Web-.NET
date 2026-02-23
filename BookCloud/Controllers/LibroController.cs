using BookCloud.Helpers;
using BookCloud.Models;
using BookCloud.Repositories;
using Microsoft.AspNetCore.Mvc;
using static BookCloud.Helpers.FolderHelper;

namespace BookCloud.Controllers
{
    public class LibroController : Controller
    {
        private RepositoryLibros repo;
        private FotoLibro _fotoHelper;
        public LibroController(RepositoryLibros repo, FotoLibro fotoHelper)
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
            return View(libro);
        }
        public IActionResult Create()
        {
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
                string path = this._fotoHelper.MapPath(imagenFile.FileName, Folder.Usuarios, UsuarioId, Titulo);
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

            await repo.InsertLibro(libro);
            return RedirectToAction("Details", new { id = libro.Id });
        }
    }
}