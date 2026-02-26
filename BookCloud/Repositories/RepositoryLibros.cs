using BookCloud.Data;
using BookCloud.Models;
using BookCloud.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;

namespace BookCloud.Repositories
{

    #region PRODIMIENTOS ALMACENADOS

    //    procedimiento para recoger libros disponibles
    //Solo muestra libros cuyo vendedor también esté activo.
    //Devuelve los libros activos con stock disponible.

    //CREATE OR ALTER PROCEDURE SP_Libros_Disponibles
    //AS
    //BEGIN
    //    SET NOCOUNT ON;

    //SELECT
    //    L.Id,
    //    L.Titulo,
    //    L.Autor,
    //    L.Descripcion,
    //    L.Precio,
    //    L.Stock,
    //    L.Foto,
    //    L.FechaPublicacion,
    //    L.Activo,
    //    U.Nombre AS Vendedor
    //FROM Libros L

    //INNER JOIN Usuarios U ON L.UsuarioId = U.Id

    //WHERE
    //    L.Activo = 1

    //    AND U.Activo = 1

    //    AND L.Stock > 0;
    //END




    //    -- Obtiene un libro específico por su Id.
    //-- Solo devuelve el libro si está activo y su vendedor también está activo.

    //CREATE OR ALTER PROCEDURE sp_Libro_ObtenerPorId
    //(
    //    @LibroId INT
    //)
    //AS
    //BEGIN
    //    SET NOCOUNT ON;

    //    SELECT
    //        L.Id,
    //        L.Titulo,
    //        L.Autor,
    //        L.Descripcion,
    //        L.Precio,
    //        L.Stock,
    //        L.Foto,
    //        L.FechaPublicacion,
    //        L.Activo,
    //        L.UsuarioId
    //        U.Nombre AS Vendedor
    //    FROM Libros L
    //    INNER JOIN Usuarios U ON L.UsuarioId = U.Id
    //    WHERE
    //        L.Id = @LibroId
    //        AND L.Activo = 1
    //        AND U.Activo = 1;
    //END



    //--procedimiento crearr Libro:
    //-- Inserta un nuevo libro en el sistema.
    //-- Valida que el usuario vendedor exista y esté activo antes de crear el registro.
    //public async Task<int> InsertLibro(Libro libro)
    //{
    //    string sql = "SP_Libro_Crear";
    //    SqlParameter panTitulo = new SqlParameter("@Titulo", libro.Titulo);
    //    SqlParameter panAutor = new SqlParameter("@Autor", libro.Autor);
    //    SqlParameter panDescripcion = new SqlParameter("@Descripcion", libro.Descripcion);
    //    SqlParameter panPrecio = new SqlParameter("@Precio", libro.Precio);
    //    SqlParameter panStock = new SqlParameter("@Stock", libro.Stock);
    //    SqlParameter panFoto = new SqlParameter("@Foto", libro.Foto);
    //    SqlParameter panFechaPublicacion = new SqlParameter("@FechaPublicacion", libro.FechaPublicacion);
    //    SqlParameter panUsuarioId = new SqlParameter("@UsuarioId", libro.UsuarioId);

    //    // Parámetro de salida para capturar el ID generado
    //    SqlParameter panLibroId = new SqlParameter("@LibroId", SqlDbType.Int)
    //    {
    //        Direction = ParameterDirection.Output
    //    };

    //    using (DbCommand com = this.context.Database.GetDbConnection().CreateCommand())
    //    {
    //        com.CommandType = CommandType.StoredProcedure;
    //        com.CommandText = sql;
    //        com.Parameters.Add(panTitulo);
    //        com.Parameters.Add(panAutor);
    //        com.Parameters.Add(panDescripcion);
    //        com.Parameters.Add(panPrecio);
    //        com.Parameters.Add(panStock);
    //        com.Parameters.Add(panFoto);
    //        com.Parameters.Add(panFechaPublicacion);
    //        com.Parameters.Add(panUsuarioId);
    //        com.Parameters.Add(panLibroId);

    //        await com.Connection.OpenAsync();
    //        await com.ExecuteNonQueryAsync();

    //        // Capturar el valor del parámetro de salida
    //        int libroId = (int)panLibroId.Value;

    //        await com.Connection.CloseAsync();
    //        com.Parameters.Clear();

    //        // Asignar el ID al objeto libro y devolverlo
    //        libro.Id = libroId;
    //        return libroId;
    //    }
    //}




    //    -- Actualiza los datos de un libro existente.
    //-- Valida que el libro esté activo antes de modificarlo.
    //CREATE OR ALTER PROCEDURE sp_Libro_Actualizar
    //(
    //    @LibroId INT,
    //    @Titulo NVARCHAR(200),
    //    @Autor NVARCHAR(200),
    //    @Descripcion NVARCHAR(MAX),
    //    @Precio DECIMAL(10,2),
    //    @Stock INT,
    //    @Foto NVARCHAR(500),
    //    @FechaPublicacion DATE
    //)
    //AS
    //BEGIN
    //    SET NOCOUNT ON;

    //    BEGIN TRY

    //        IF NOT EXISTS(
    //            SELECT 1 
    //            FROM Libros
    //            WHERE Id = @LibroId
    //            AND Activo = 1
    //        )
    //        BEGIN
    //            RAISERROR('El libro no existe o está inactivo.',16,1);
    //RETURN;
    //        END

    //        UPDATE Libros
    //        SET
    //            Titulo = @Titulo,
    //            Autor = @Autor,
    //            Descripcion = @Descripcion,
    //            Precio = @Precio,
    //            Stock = @Stock,
    //            Foto = @Foto,
    //            FechaPublicacion = @FechaPublicacion
    //        WHERE Id = @LibroId;

    //    END TRY
    //    BEGIN CATCH
    //        THROW;
    //    END CATCH
    //END

    //-- Desactiva un libro cambiando su estado Activo a 0.
    //-- No elimina físicamente el registro para mantener integridad histórica.

    //CREATE OR ALTER PROCEDURE sp_Libro_Eliminar
    //(
    //    @LibroId INT
    //)
    //AS
    //BEGIN
    //    SET NOCOUNT ON;

    //    BEGIN TRY

    //        IF NOT EXISTS(
    //            SELECT 1 
    //            FROM Libros
    //            WHERE Id = @LibroId
    //            AND Activo = 1
    //        )
    //        BEGIN
    //            RAISERROR('El libro no existe o ya está inactivo.',16,1);
    //RETURN;
    //        END

    //        UPDATE Libros
    //        SET Activo = 0
    //        WHERE Id = @LibroId;

    //END TRY
    //    BEGIN CATCH
    //        THROW;
    //    END CATCH
    //END



    #endregion

    public class RepositoryLibros : IRepositoryLibros
    {
        private BookCloudContext context;

        public RepositoryLibros(BookCloudContext context)
        {
            this.context = context;
        }

        public async Task<List<Libro>> GetLibros()
        {
            string sql = "SP_Libros_Disponibles";
            List<Libro> Libros = await this.context.Libros.FromSqlRaw(sql).ToListAsync();
            return Libros;
        }

        public async Task<Libro> GetLibro(int id)
        {
            string sql = "sp_Libro_ObtenerPorId @LibroId";
            SqlParameter panLibroId = new SqlParameter("@LibroId", id);
            Libro libro = this.context.Libros.FromSqlRaw(sql, panLibroId).AsEnumerable().FirstOrDefault();

            return libro;
        }


        public async Task<int> InsertLibro(Libro libro)
        {
            string sql = "SP_Libro_Crear";
            SqlParameter panTitulo = new SqlParameter("@Titulo", libro.Titulo);
            SqlParameter panAutor = new SqlParameter("@Autor", libro.Autor);
            SqlParameter panDescripcion = new SqlParameter("@Descripcion", libro.Descripcion);
            SqlParameter panPrecio = new SqlParameter("@Precio", libro.Precio);
            SqlParameter panStock = new SqlParameter("@Stock", libro.Stock);
            SqlParameter panFoto = new SqlParameter("@Foto", libro.Foto);
            SqlParameter panFechaPublicacion = new SqlParameter("@FechaPublicacion", libro.FechaPublicacion);
            SqlParameter panUsuarioId = new SqlParameter("@UsuarioId", libro.UsuarioId);

            // Parámetro de salida para capturar el ID generado
            SqlParameter panLibroId = new SqlParameter("@LibroId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            using (DbCommand com = this.context.Database.GetDbConnection().CreateCommand())
            {
                com.CommandType = CommandType.StoredProcedure;
                com.CommandText = sql;
                com.Parameters.Add(panTitulo);
                com.Parameters.Add(panAutor);
                com.Parameters.Add(panDescripcion);
                com.Parameters.Add(panPrecio);
                com.Parameters.Add(panStock);
                com.Parameters.Add(panFoto);
                com.Parameters.Add(panFechaPublicacion);
                com.Parameters.Add(panUsuarioId);
                com.Parameters.Add(panLibroId);

                await com.Connection.OpenAsync();
                await com.ExecuteNonQueryAsync();

                // Capturar el valor del parámetro de salida
                int libroId = (int)panLibroId.Value;

                await com.Connection.CloseAsync();
                com.Parameters.Clear();

                // Asignar el ID al objeto libro y devolverlo
                libro.Id = libroId;
                return libroId;
            }
        }

        public async Task UpdateLibro(Libro libro)
        {
            string sql = "sp_Libro_Actualizar";
            SqlParameter panLibroId = new SqlParameter("@LibroId", libro.Id);
            SqlParameter panTitulo = new SqlParameter("@Titulo", libro.Titulo);
            SqlParameter panAutor = new SqlParameter("@Autor", libro.Autor);
            SqlParameter panDescripcion = new SqlParameter("@Descripcion", libro.Descripcion);
            SqlParameter panPrecio = new SqlParameter("@Precio", libro.Precio);
            SqlParameter panStock = new SqlParameter("@Stock", libro.Stock);
            SqlParameter panFoto = new SqlParameter("@Foto", libro.Foto);
            SqlParameter panFechaPublicacion = new SqlParameter("@FechaPublicacion", libro.FechaPublicacion);

            using (DbCommand com = this.context.Database.GetDbConnection().CreateCommand())
            {
                com.CommandType = CommandType.StoredProcedure;
                com.CommandText = sql;
                com.Parameters.Add(panLibroId);
                com.Parameters.Add(panTitulo);
                com.Parameters.Add(panAutor);
                com.Parameters.Add(panDescripcion);
                com.Parameters.Add(panPrecio);
                com.Parameters.Add(panStock);
                com.Parameters.Add(panFoto);
                com.Parameters.Add(panFechaPublicacion);
                await com.Connection.OpenAsync();
                await com.ExecuteNonQueryAsync();
                await com.Connection.CloseAsync();
                com.Parameters.Clear();
            }
        }
        public async Task DeleteLibro(int id)
        {
            string sql = "sp_Libro_Eliminar @LibroId";
            SqlParameter panLibroId = new SqlParameter("@LibroId", id);

            using (DbCommand com = this.context.Database.GetDbConnection().CreateCommand())
            {
                com.CommandType = CommandType.StoredProcedure;
                com.CommandText = sql;
                com.Parameters.Add(panLibroId);
                await com.Connection.OpenAsync();
                await com.ExecuteNonQueryAsync();
                await com.Connection.CloseAsync();
                com.Parameters.Clear();
            }

        }
    }
}
