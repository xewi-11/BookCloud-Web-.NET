using BookCloud.Models;
using Microsoft.EntityFrameworkCore;

namespace BookCloud.Data
{
    public class BookCloudContext : DbContext
    {
        public BookCloudContext(DbContextOptions<BookCloudContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<UsuarioSeguridad> UsuarioCredenciales { get; set; }
        public DbSet<Libro> Libros { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar relación uno a uno entre Usuario y UsuarioSeguridad
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.UsuarioSeguridad)
                .WithOne(us => us.Usuario)
                .HasForeignKey<UsuarioSeguridad>(us => us.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
