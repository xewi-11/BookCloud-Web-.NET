CREATE DATABASE BookCloud;
GO

USE BookCloud;
GO

/* =====================================================
   SEQUENCES
===================================================== */

CREATE SEQUENCE Seq_Usuarios START WITH 1 INCREMENT BY 1;
CREATE SEQUENCE Seq_Libros START WITH 1 INCREMENT BY 1;
CREATE SEQUENCE Seq_Pedidos START WITH 1 INCREMENT BY 1;
CREATE SEQUENCE Seq_Pagos START WITH 1 INCREMENT BY 1;
CREATE SEQUENCE Seq_Chats START WITH 1 INCREMENT BY 1;
CREATE SEQUENCE Seq_Mensajes START WITH 1 INCREMENT BY 1;
CREATE SEQUENCE Seq_Favoritos START WITH 1 INCREMENT BY 1;
CREATE SEQUENCE Seq_PedidoDetalle START WITH 1 INCREMENT BY 1;
CREATE SEQUENCE Seq_SaldoMovimientos START WITH 1 INCREMENT BY 1;
CREATE SEQUENCE Seq_UsuarioCredenciales START WITH 1 INCREMENT BY 1;
GO

/* =====================================================
   TABLA: Usuarios
===================================================== */

CREATE TABLE Usuarios (
    Id INT PRIMARY KEY DEFAULT NEXT VALUE FOR Seq_Usuarios,
    Nombre NVARCHAR(100) NOT NULL,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    Password NVARCHAR(50) NOT NULL,
    FotoUrl NVARCHAR(300) NULL,
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE(),
    Activo BIT NOT NULL DEFAULT 1
);
GO

CREATE INDEX IX_Usuarios_Activo
ON Usuarios(Activo)
WHERE Activo = 1;
GO


CREATE TABLE UsuarioCredenciales (
    
    Id INT PRIMARY KEY 
        DEFAULT NEXT VALUE FOR Seq_UsuarioCredenciales,

    UsuarioId INT NOT NULL,

    PasswordHash VARBINARY(MAX) NOT NULL,

    Salt NVARCHAR(150) NOT NULL,


    Activo BIT NOT NULL DEFAULT 1,

    CONSTRAINT FK_UsuarioCredenciales_Usuarios
        FOREIGN KEY (UsuarioId)
        REFERENCES Usuarios(Id)
        ON DELETE CASCADE,

    CONSTRAINT UQ_UsuarioCredenciales_Usuario
        UNIQUE (UsuarioId)  -- Un usuario solo tiene una credencial activa
);
GO

CREATE INDEX IX_UsuarioCredenciales_UsuarioId
ON UsuarioCredenciales(UsuarioId);
GO

/* =====================================================
   TABLA: Libros
===================================================== */

CREATE TABLE Libros (
    Id INT PRIMARY KEY DEFAULT NEXT VALUE FOR Seq_Libros,
    Titulo NVARCHAR(200) NOT NULL,
    Autor NVARCHAR(150) NOT NULL,
    Descripcion NVARCHAR(MAX),
    Foto NVARCHAR(300) NULL,
    Precio DECIMAL(10,2) NOT NULL,
    Stock INT NOT NULL DEFAULT 0,
    UsuarioId INT NOT NULL,
    FechaPublicacion DATETIME NOT NULL DEFAULT GETDATE(),
    Activo BIT NOT NULL DEFAULT 1,

    CONSTRAINT FK_Libros_Usuarios
        FOREIGN KEY (UsuarioId)
        REFERENCES Usuarios(Id)
        ON DELETE NO ACTION
);
GO

CREATE INDEX IX_Libros_UsuarioId ON Libros(UsuarioId);
CREATE INDEX IX_Libros_Titulo ON Libros(Titulo);
CREATE INDEX IX_Libros_Activo ON Libros(Activo) WHERE Activo = 1;
GO

/* =====================================================
   TABLA: Favoritos
===================================================== */

CREATE TABLE Favoritos (
    Id INT PRIMARY KEY DEFAULT NEXT VALUE FOR Seq_Favoritos,
    UsuarioId INT NOT NULL,
    LibroId INT NOT NULL,
    FechaAgregado DATETIME NOT NULL DEFAULT GETDATE(),
    Activo BIT NOT NULL DEFAULT 1,

    CONSTRAINT FK_Favoritos_Usuarios
        FOREIGN KEY (UsuarioId)
        REFERENCES Usuarios(Id)
        ON DELETE NO ACTION,

    CONSTRAINT FK_Favoritos_Libros
        FOREIGN KEY (LibroId)
        REFERENCES Libros(Id)
        ON DELETE NO ACTION,

    CONSTRAINT UQ_Favorito_Usuario_Libro
        UNIQUE (UsuarioId, LibroId)
);
GO

CREATE INDEX IX_Favoritos_UsuarioId ON Favoritos(UsuarioId);
CREATE INDEX IX_Favoritos_LibroId ON Favoritos(LibroId);
CREATE INDEX IX_Favoritos_Activo ON Favoritos(Activo) WHERE Activo = 1;
GO

/* =====================================================
   TABLA: Pedidos
===================================================== */

CREATE TABLE Pedidos (
    Id INT PRIMARY KEY DEFAULT NEXT VALUE FOR Seq_Pedidos,
    UsuarioId INT NOT NULL,
    FechaPedido DATETIME NOT NULL DEFAULT GETDATE(),
    Total DECIMAL(10,2) NOT NULL,
    Estado NVARCHAR(50) NOT NULL,
    Activo BIT NOT NULL DEFAULT 1,

    CONSTRAINT FK_Pedidos_Usuarios
        FOREIGN KEY (UsuarioId)
        REFERENCES Usuarios(Id)
        ON DELETE NO ACTION
);
GO

CREATE INDEX IX_Pedidos_UsuarioId ON Pedidos(UsuarioId);
CREATE INDEX IX_Pedidos_Activo ON Pedidos(Activo) WHERE Activo = 1;
GO

/* =====================================================
   TABLA: PedidoDetalle
===================================================== */

CREATE TABLE PedidoDetalle (
    Id INT PRIMARY KEY DEFAULT NEXT VALUE FOR Seq_PedidoDetalle,
    PedidoId INT NOT NULL,
    LibroId INT NOT NULL,
    Cantidad INT NOT NULL CHECK (Cantidad > 0),
    PrecioUnitario DECIMAL(10,2) NOT NULL CHECK (PrecioUnitario >= 0),
    Activo BIT NOT NULL DEFAULT 1,

    CONSTRAINT FK_PedidoDetalle_Pedidos
        FOREIGN KEY (PedidoId)
        REFERENCES Pedidos(Id)
        ON DELETE NO ACTION,

    CONSTRAINT FK_PedidoDetalle_Libros
        FOREIGN KEY (LibroId)
        REFERENCES Libros(Id)
        ON DELETE NO ACTION,

    CONSTRAINT UQ_Pedido_Libro
        UNIQUE (PedidoId, LibroId)
);
GO

CREATE INDEX IX_PedidoDetalle_PedidoId ON PedidoDetalle(PedidoId);
CREATE INDEX IX_PedidoDetalle_LibroId ON PedidoDetalle(LibroId);
CREATE INDEX IX_PedidoDetalle_Activo ON PedidoDetalle(Activo) WHERE Activo = 1;
GO

/* =====================================================
   TABLA: Pagos
===================================================== */

CREATE TABLE Pagos (
    Id INT PRIMARY KEY DEFAULT NEXT VALUE FOR Seq_Pagos,
    PedidoId INT NOT NULL,
    FechaPago DATETIME NOT NULL DEFAULT GETDATE(),
    Monto DECIMAL(10,2) NOT NULL,
    Metodo NVARCHAR(50) NOT NULL DEFAULT 'Tarjeta',
    Estado NVARCHAR(50) NOT NULL,
    Activo BIT NOT NULL DEFAULT 1,

    CONSTRAINT FK_Pagos_Pedidos
        FOREIGN KEY (PedidoId)
        REFERENCES Pedidos(Id)
        ON DELETE NO ACTION
);
GO

CREATE INDEX IX_Pagos_PedidoId ON Pagos(PedidoId);
CREATE INDEX IX_Pagos_Activo ON Pagos(Activo) WHERE Activo = 1;
GO

/* =====================================================
   TABLA: SaldoMovimientos
===================================================== */

CREATE TABLE SaldoMovimientos (
    Id INT PRIMARY KEY DEFAULT NEXT VALUE FOR Seq_SaldoMovimientos,
    UsuarioId INT NOT NULL,
    PedidoId INT NULL,
    Monto DECIMAL(10,2) NOT NULL,
    Tipo NVARCHAR(50) NOT NULL,
    Descripcion NVARCHAR(255) NULL,
    Fecha DATETIME NOT NULL DEFAULT GETDATE(),
    Activo BIT NOT NULL DEFAULT 1,

    CONSTRAINT FK_SaldoMovimientos_Usuarios
        FOREIGN KEY (UsuarioId)
        REFERENCES Usuarios(Id)
        ON DELETE NO ACTION,

    CONSTRAINT FK_SaldoMovimientos_Pedidos
        FOREIGN KEY (PedidoId)
        REFERENCES Pedidos(Id)
        ON DELETE NO ACTION
);
GO

CREATE INDEX IX_SaldoMovimientos_UsuarioId ON SaldoMovimientos(UsuarioId);
CREATE INDEX IX_SaldoMovimientos_PedidoId ON SaldoMovimientos(PedidoId);
CREATE INDEX IX_SaldoMovimientos_Activo ON SaldoMovimientos(Activo);
GO

/* =====================================================
   TABLA: Chats
===================================================== */

CREATE TABLE Chats (
    Id INT PRIMARY KEY DEFAULT NEXT VALUE FOR Seq_Chats,
    Usuario1Id INT NOT NULL,
    Usuario2Id INT NOT NULL,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    Activo BIT NOT NULL DEFAULT 1,

    CONSTRAINT FK_Chats_Usuario1
        FOREIGN KEY (Usuario1Id)
        REFERENCES Usuarios(Id)
        ON DELETE NO ACTION,

    CONSTRAINT FK_Chats_Usuario2
        FOREIGN KEY (Usuario2Id)
        REFERENCES Usuarios(Id)
        ON DELETE NO ACTION
);
GO

CREATE INDEX IX_Chats_Usuario1Id ON Chats(Usuario1Id);
CREATE INDEX IX_Chats_Usuario2Id ON Chats(Usuario2Id);
CREATE INDEX IX_Chats_Activo ON Chats(Activo) WHERE Activo = 1;
GO

/* =====================================================
   TABLA: Mensajes
===================================================== */

CREATE TABLE Mensajes (
    Id INT PRIMARY KEY DEFAULT NEXT VALUE FOR Seq_Mensajes,
    ChatId INT NOT NULL,
    RemitenteId INT NOT NULL,
    Contenido NVARCHAR(MAX) NOT NULL,
    FechaEnvio DATETIME NOT NULL DEFAULT GETDATE(),
    Activo BIT NOT NULL DEFAULT 1,

    CONSTRAINT FK_Mensajes_Chats
        FOREIGN KEY (ChatId)
        REFERENCES Chats(Id)
        ON DELETE NO ACTION,

    CONSTRAINT FK_Mensajes_Usuarios
        FOREIGN KEY (RemitenteId)
        REFERENCES Usuarios(Id)
        ON DELETE NO ACTION
);
GO

CREATE INDEX IX_Mensajes_ChatId ON Mensajes(ChatId);
CREATE INDEX IX_Mensajes_RemitenteId ON Mensajes(RemitenteId);
CREATE INDEX IX_Mensajes_Activo ON Mensajes(Activo) WHERE Activo = 1;
GO

--procedimiento creare pedido:
-- Crea un pedido a partir del carrito del usuario.
-- Valida stock, descuenta inventario de forma segura y elimina el carrito dentro de una transacción.
CREATE OR ALTER PROCEDURE SP_Pedido_Crear
(
    @UsuarioId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Validar usuario activo
        IF NOT EXISTS (
            SELECT 1 FROM Usuarios 
            WHERE Id = @UsuarioId AND Activo = 1
        )
            RAISERROR('Usuario inválido.',16,1);

        -- Validar carrito
        IF NOT EXISTS (
            SELECT 1 FROM Carrito 
            WHERE UsuarioId = @UsuarioId
        )
            RAISERROR('El carrito está vacío.',16,1);

        -- Verificar stock ANTES de crear pedido
        IF EXISTS (
            SELECT 1
            FROM Carrito C
            INNER JOIN Libros L ON C.LibroId = L.Id
            WHERE C.UsuarioId = @UsuarioId
            AND L.Stock < C.Cantidad
        )
            RAISERROR('Stock insuficiente para uno o más libros.',16,1);

        DECLARE @Total DECIMAL(18,2);

        SELECT @Total = SUM(L.Precio * C.Cantidad)
        FROM Carrito C
        INNER JOIN Libros L ON C.LibroId = L.Id
        WHERE C.UsuarioId = @UsuarioId;

        INSERT INTO Pedidos (UsuarioId, FechaPedido, Total, Estado, Activo)
        VALUES (@UsuarioId, GETDATE(), @Total, 'Pendiente', 1);

        DECLARE @PedidoId INT = SCOPE_IDENTITY();

        INSERT INTO PedidoDetalle (PedidoId, LibroId, Cantidad, PrecioUnitario)
        SELECT 
            @PedidoId,
            C.LibroId,
            C.Cantidad,
            L.Precio
        FROM Carrito C
        INNER JOIN Libros L ON C.LibroId = L.Id
        WHERE C.UsuarioId = @UsuarioId;

        -- 🔥 DESCUENTO SEGURO DE STOCK
        UPDATE L
        SET L.Stock = L.Stock - C.Cantidad
        FROM Libros L
        INNER JOIN Carrito C ON L.Id = C.LibroId
        WHERE C.UsuarioId = @UsuarioId
        AND L.Stock >= C.Cantidad;

        -- Validar que todas las filas se actualizaron
        IF @@ROWCOUNT <> (
            SELECT COUNT(*) FROM Carrito WHERE UsuarioId = @UsuarioId
        )
        BEGIN
            RAISERROR('Error de concurrencia: stock insuficiente.',16,1);
        END

        DELETE FROM Carrito
        WHERE UsuarioId = @UsuarioId;

        COMMIT;

        SELECT @PedidoId AS PedidoId;

    END TRY
    BEGIN CATCH
        ROLLBACK;
        THROW;
    END CATCH
END


--procedimiento agregar carrito:
--Agrega un libro al carrito del usuario.
--Valida usuario activo y stock disponible; si ya existe, aumenta la cantidad.
CREATE OR ALTER PROCEDURE SP_Carrito_Agregar
(
    @UsuarioId INT,
    @LibroId INT,
    @Cantidad INT
)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF @Cantidad <= 0
            RAISERROR('Cantidad inválida.',16,1);

        IF NOT EXISTS (
            SELECT 1 FROM Usuarios 
            WHERE Id = @UsuarioId AND Activo = 1
        )
            RAISERROR('Usuario inválido.',16,1);

        DECLARE @StockActual INT;

        SELECT @StockActual = Stock
        FROM Libros
        WHERE Id = @LibroId AND Activo = 1;

        IF @StockActual IS NULL
            RAISERROR('Libro no disponible.',16,1);

        IF @StockActual < @Cantidad
            RAISERROR('Stock insuficiente.',16,1);

        IF EXISTS (
            SELECT 1 FROM Carrito
            WHERE UsuarioId = @UsuarioId
              AND LibroId = @LibroId
        )
        BEGIN
            UPDATE Carrito
            SET Cantidad = Cantidad + @Cantidad
            WHERE UsuarioId = @UsuarioId
              AND LibroId = @LibroId;
        END
        ELSE
        BEGIN
            INSERT INTO Carrito (UsuarioId, LibroId, Cantidad)
            VALUES (@UsuarioId, @LibroId, @Cantidad);
        END

    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END



    CREATE OR ALTER PROCEDURE SP_Libros_Disponibles
    AS
    BEGIN
        SET NOCOUNT ON;

    SELECT
        L.Id,
        L.Titulo,
        L.Autor,
        L.Descripcion,
        L.Precio,
        L.Stock,
        L.Foto,
        L.FechaPublicacion,
        L.Activo,
        L.UsuarioId,
        U.Nombre AS Vendedor
    FROM Libros L

    INNER JOIN Usuarios U ON L.UsuarioId = U.Id

    WHERE
        L.Activo = 1

        AND U.Activo = 1

        AND L.Stock > 0;
    END




        -- Obtiene un libro específico por su Id.
    -- Solo devuelve el libro si está activo y su vendedor también está activo.

    CREATE OR ALTER PROCEDURE sp_Libro_ObtenerPorId
    (
        @LibroId INT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        SELECT
            L.Id,
            L.Titulo,
            L.Autor,
            L.Descripcion,
            L.Precio,
            L.Stock,
            L.Foto,
            L.FechaPublicacion,
            L.Activo,
            L.UsuarioId,
            U.Nombre AS Vendedor
        FROM Libros L
        INNER JOIN Usuarios U ON L.UsuarioId = U.Id
        WHERE
            L.Id = @LibroId
            AND L.Activo = 1
            AND U.Activo = 1;
    END



    --procedimiento crearr Libro:
    -- Inserta un nuevo libro en el sistema.
    -- Valida que el usuario vendedor exista y esté activo antes de crear el registro.
    CREATE OR ALTER PROCEDURE SP_Libro_Crear
    (
        @Titulo NVARCHAR(200),
        @Autor NVARCHAR(200),
        @Descripcion NVARCHAR(MAX),
        @Precio DECIMAL(10,2),
        @Stock INT,
        @Foto NVARCHAR(500),
        @FechaPublicacion DATE,
        @UsuarioId INT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        IF NOT EXISTS(
            SELECT 1 FROM Usuarios
            WHERE Id = @UsuarioId AND Activo = 1
        )
        BEGIN
            RAISERROR('El usuario no existe o no está activo.',16,1);
    RETURN;
        END

        INSERT INTO Libros
        (
            Titulo,
            Autor,
            Descripcion,
            Precio,
            Stock,
            Foto,
            FechaPublicacion,
            UsuarioId,
            Activo
        )
        VALUES
        (
            @Titulo,
            @Autor,
            @Descripcion,
            @Precio,
            @Stock,
            @Foto,
            @FechaPublicacion,
            @UsuarioId,
            1
        );
    END




        -- Actualiza los datos de un libro existente.
    -- Valida que el libro esté activo antes de modificarlo.
    CREATE OR ALTER PROCEDURE sp_Libro_Actualizar
    (
        @LibroId INT,
        @Titulo NVARCHAR(200),
        @Autor NVARCHAR(200),
        @Descripcion NVARCHAR(MAX),
        @Precio DECIMAL(10,2),
        @Stock INT,
        @Foto NVARCHAR(500),
        @FechaPublicacion DATE
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        BEGIN TRY

            IF NOT EXISTS(
                SELECT 1 
                FROM Libros
                WHERE Id = @LibroId
                AND Activo = 1
            )
            BEGIN
                RAISERROR('El libro no existe o está inactivo.',16,1);
    RETURN;
            END

            UPDATE Libros
            SET
                Titulo = @Titulo,
                Autor = @Autor,
                Descripcion = @Descripcion,
                Precio = @Precio,
                Stock = @Stock,
                Foto = @Foto,
                FechaPublicacion = @FechaPublicacion
            WHERE Id = @LibroId;

        END TRY
        BEGIN CATCH
            THROW;
        END CATCH
    END

    -- Desactiva un libro cambiando su estado Activo a 0.
    -- No elimina físicamente el registro para mantener integridad histórica.

    CREATE OR ALTER PROCEDURE sp_Libro_Eliminar
    (
        @LibroId INT
    )
    AS
    BEGIN
        SET NOCOUNT ON;

        BEGIN TRY

            IF NOT EXISTS(
                SELECT 1 
                FROM Libros
                WHERE Id = @LibroId
                AND Activo = 1
            )
            BEGIN
                RAISERROR('El libro no existe o ya está inactivo.',16,1);
    RETURN;
            END

            UPDATE Libros
            SET Activo = 0
            WHERE Id = @LibroId;

    END TRY
        BEGIN CATCH
            THROW;
        END CATCH
    END