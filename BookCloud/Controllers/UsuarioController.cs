using BookCloud.Models;
using BookCloud.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BookCloud.Controllers
{
    public class UsuarioController : Controller
    {
        private RepositoryUsuarios _repo;
        public UsuarioController(RepositoryUsuarios repo)
        {
            _repo = repo;
        }
        public async Task<IActionResult> IndexPerfil()
        {
            Usuario user = await this._repo.GetInfoUsario(HttpContext.Session.GetString("Id"));
            return View(user);
        }
    }
}
