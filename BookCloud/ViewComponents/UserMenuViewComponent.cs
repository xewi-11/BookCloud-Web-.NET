using BookCloud.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace BookCloud.ViewComponents
{
    public class UserMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            // 🆕 MIGRADO A CLAIMS: Leer desde cookie de autenticación
            var userId = AuthHelper.GetUserId(UserClaimsPrincipal);
            var userName = AuthHelper.GetUserName(UserClaimsPrincipal);

            // Pasa los datos a la vista del componente
            ViewData["UserId"] = userId?.ToString();
            ViewData["UserName"] = userName;
            ViewData["IsAuthenticated"] = AuthHelper.IsAuthenticated(UserClaimsPrincipal);

            return View();
        }
    }
}

