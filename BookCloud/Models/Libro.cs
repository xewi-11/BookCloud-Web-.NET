using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookCloud.Models
{
    [Table("Libros")]
    public class Libro
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }


        [Column("Titulo")]
        public string Titulo { get; set; }


        [Column("Autor")]
        public string Autor { get; set; }

        [Column("Descripcion")]
        public string? Descripcion { get; set; }
        [Column("Foto")]
        public string? Foto { get; set; }


        [Column("Precio")]
        public decimal Precio { get; set; }


        [Column("Stock")]
        public int Stock { get; set; }


        [Column("UsuarioId")]
        public int UsuarioId { get; set; }

        [Column("FechaPublicacion")]
        public DateTime FechaPublicacion { get; set; }


        [Column("Activo")]
        public bool Activo { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }
    }
}
