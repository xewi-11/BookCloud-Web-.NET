using System.Security.Claims;

namespace BookCloud.Helpers
{
    /// <summary>
    /// Helper para acceder a los datos del usuario autenticado desde Claims (Cookie de autenticación)
    /// Reemplaza el uso de Session para datos de usuario
    /// </summary>
    public static class AuthHelper
    {
        /// <summary>
        /// Obtiene el ID del usuario autenticado desde los Claims
        /// </summary>
        public static int? GetUserId(ClaimsPrincipal user)
        {
            var claim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null && int.TryParse(claim.Value, out int userId))
            {
                return userId;
            }
            return null;
        }

        /// <summary>
        /// Obtiene el nombre del usuario autenticado desde los Claims
        /// </summary>
        public static string? GetUserName(ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }

        /// <summary>
        /// Obtiene el correo del usuario autenticado desde los Claims
        /// </summary>
        public static string? GetUserEmail(ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Email)?.Value;
        }

        /// <summary>
        /// Obtiene la foto del usuario autenticado desde los Claims
        /// </summary>
        public static string? GetUserPhoto(ClaimsPrincipal user)
        {
            return user.FindFirst("Foto")?.Value;
        }

        /// <summary>
        /// Verifica si el usuario está autenticado
        /// </summary>
        public static bool IsAuthenticated(ClaimsPrincipal user)
        {
            return user?.Identity?.IsAuthenticated ?? false;
        }
    }
}
