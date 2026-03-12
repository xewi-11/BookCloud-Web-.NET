using BookCloud.Helpers;
using BookCloud.Models;
using BookCloud.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BookCloud.Controllers
{
    public class AuthController : Controller
    {
        private IRepositoryUsuarios _repo;

        public AuthController(IRepositoryUsuarios repo)
        {
            this._repo = repo;
        }

        [HttpGet]
        public IActionResult RegisterUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterUSer usuario)
        {
            if (usuario == null)
            {
                return View();
            }

            string salt = Encryption.GenerateSalt();
            byte[] encryptedPassword = Encryption.EncryptPassword(usuario.Pass, salt);

            Usuario user = new Usuario
            {
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                Password = usuario.Pass, // Contraseña en texto plano opcional
                FechaRegistro = DateTime.Now,
                Activo = true,
                Foto = null
            };

            UsuarioSeguridad seguridad = new UsuarioSeguridad
            {
                PasswordHash = encryptedPassword,
                Salt = salt
            };

            await this._repo.CreateUserASync(user, seguridad);
            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Credenciales cred)
        {
            Usuario s = await this._repo.GetUserByEmail(cred.Correo);
            if (s != null && s.UsuarioSeguridad != null)
            {
                // Verificar la contraseña
                byte[] hash = Encryption.EncryptPassword(cred.Password, s.UsuarioSeguridad.Salt);
                if (Encryption.CompareArrays(hash, s.UsuarioSeguridad.PasswordHash))
                {
                    // 🆕 MIGRADO A CLAIMS: Ya no usamos Session para datos de usuario
                    // Crear Claims con toda la información del usuario
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, s.Id.ToString()),
                        new Claim(ClaimTypes.Name, s.Nombre),
                        new Claim(ClaimTypes.Email, s.Correo)
                    };

                    // Agregar foto si existe
                    if (!string.IsNullOrEmpty(s.Foto))
                    {
                        claims.Add(new Claim("Foto", s.Foto));
                    }

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToAction("Index", "Libro");
                }
            }
            // Autenticación fallida
            ViewData["Error"] = "Correo o contraseña incorrectos.";
            return View();
        }


        public async Task<IActionResult> Logout()
        {
            // 🆕 MIGRADO A CLAIMS: Limpiar cookie de autenticación (ya no usamos Session para usuario)
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // ✅ Limpiar Session (mantener por si hay carrito u otros datos)
            HttpContext.Session.Clear();

            // Redirigir a la página de login
            return RedirectToAction("Login");
        }
    }
}