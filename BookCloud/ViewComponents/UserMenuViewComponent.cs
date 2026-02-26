using Microsoft.AspNetCore.Mvc;

namespace BookCloud.ViewComponents
{
    public class UserMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            // Lee la sesión en cada petición HTTP
            var userId = HttpContext.Session.GetString("Id");
            var userName = HttpContext.Session.GetString("Nombre");

            // Pasa los datos a la vista del componente
            ViewData["UserId"] = userId;
            ViewData["UserName"] = userName;
            ViewData["IsAuthenticated"] = !string.IsNullOrEmpty(userId);

            return View();
        }
    }
}
