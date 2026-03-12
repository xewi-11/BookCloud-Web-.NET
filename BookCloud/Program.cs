using BookCloud.Data;
using BookCloud.Helpers;
using BookCloud.Hubs; // ✅ Agregar
using BookCloud.Repositories;
using BookCloud.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies; // 🆕 SEGURIDAD ADICIONAL

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDistributedMemoryCache();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<FotoUsuario>();
builder.Services.AddSingleton<FolderHelper>();
builder.Services.AddSingleton<FotoLibro>();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
});

builder.Services.AddTransient<IRepositoryUsuarios, RepositoryUsuarios>();
builder.Services.AddTransient<IRepositoryLibros, RepositoryLibros>();
builder.Services.AddTransient<IRepositoryWallet, RepositoryWallet>();
builder.Services.AddTransient<IRepositoryPedidos, RepositoryPedidos>();
builder.Services.AddScoped<IRepositoryPagos, RepositoryPagos>();
builder.Services.AddTransient<IRepositoryFavoritos, RepositoryFavoritos>();
builder.Services.AddTransient<IRepositoryChats, RepositoryChats>(); // ✅ Agregar

builder.Services.AddDbContext<BookCloudContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BookCloud")));

// 🆕 SEGURIDAD ADICIONAL: Configurar autenticación por cookies
// NOTA: Esto NO reemplaza tu sistema de sesión, solo añade una capa adicional de seguridad
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "BookCloud.Auth";
        options.Cookie.HttpOnly = true; // No accesible desde JavaScript (protege contra XSS)
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Solo HTTPS
        options.Cookie.SameSite = SameSiteMode.Strict; // Protege contra CSRF
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/Login";
    });

// ✅ Agregar SignalR
builder.Services.AddSignalR();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Archivos estáticos (CSS, JS, imágenes)
app.UseRouting(); // Enrutamiento

// 🔐 SEGURIDAD: Orden crítico del middleware
app.UseSession(); // Session debe ir antes de Authentication
app.UseAuthentication(); // Autenticación basada en cookies
app.UseAuthorization(); // Autorización (valida permisos)

app.MapStaticAssets();

// 🆕 EQUIVALENTE A UseMvc en .NET 10
// Este es el enfoque moderno que reemplaza UseMvc (obsoleto)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}")
    .WithStaticAssets();

// Nota: En .NET Framework/Core 2.x se usaba:
// app.UseMvc(routes => {
//     routes.MapRoute(name: "default", template: "{controller=Auth}/{action=Login}/{id?}");
// });
// Pero UseMvc está obsoleto desde .NET Core 3.0 y no existe en .NET 5+

// ✅ Mapear el Hub de SignalR
app.MapHub<ChatHub>("/chatHub");

app.Run();
