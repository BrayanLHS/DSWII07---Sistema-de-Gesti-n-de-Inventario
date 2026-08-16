-- ============================================
-- Script de base de datos: SistemaInventario
-- Ejecutar en SQL Server (SSMS) conectado a: DESKTOP-U5QC7VO\MISQLSERVER
-- ============================================

IF DB_ID('SistemaInventario') IS NULL
BEGIN
    CREATE DATABASE SistemaInventario;
END;
GO

USE SistemaInventario;
GO

CREATE TABLE Categoria
(
    IdCategoria INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL
);
GO

CREATE TABLE Proveedor
(
    IdProveedor INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(150) NOT NULL,
    Contacto VARCHAR(100) NULL,
    Telefono VARCHAR(20) NULL
);
GO

CREATE TABLE Producto
(
    IdProducto INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(150) NOT NULL,
    IdCategoria INT NOT NULL,
    IdProveedor INT NULL,
    Precio DECIMAL(10,2) NOT NULL,
    Stock INT NOT NULL DEFAULT 0,

    FOREIGN KEY (IdCategoria)
        REFERENCES Categoria(IdCategoria),

    FOREIGN KEY (IdProveedor)
        REFERENCES Proveedor(IdProveedor)
        ON DELETE SET NULL,

    CHECK (Precio > 0),
    CHECK (Stock >= 0)
);
GO

CREATE TABLE MovimientoInventario
(
    IdMovimiento INT IDENTITY(1,1) PRIMARY KEY,
    IdProducto INT NOT NULL,
    Tipo VARCHAR(10) NOT NULL,
    Cantidad INT NOT NULL,
    Fecha DATETIME2 NOT NULL DEFAULT GETDATE(),
    Motivo VARCHAR(250) NULL,

    FOREIGN KEY (IdProducto)
        REFERENCES Producto(IdProducto)
        ON DELETE CASCADE,

    CHECK (Tipo IN ('Entrada', 'Salida')),
    CHECK (Cantidad > 0)
);
GO

CREATE TABLE Usuario
(
    IdUsuario INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Apellido VARCHAR(100) NOT NULL,
    Correo VARCHAR(100) NOT NULL UNIQUE,
    Clave VARCHAR(255) NOT NULL,
    Rol VARCHAR(20) NOT NULL DEFAULT 'Usuario',

    CHECK (Rol IN ('Admin', 'Usuario'))
);
GO

-- ============================================
-- Datos de ejemplo
-- ============================================

INSERT INTO Categoria (Nombre)
VALUES
('Computadoras'),
('Perifericos'),
('Componentes'),
('Almacenamiento');
GO

INSERT INTO Proveedor (Nombre, Contacto, Telefono)
VALUES
('TecnoPeru', 'Carlos Perez', '987654321'),
('Importaciones Digitales', 'Maria Lopez', '912345678');
GO

INSERT INTO Producto
(Nombre, IdCategoria, IdProveedor, Precio, Stock)
VALUES
('Laptop Lenovo IdeaPad', 1, 1, 2499.90, 8),
('Mouse Logitech G203', 2, 2, 89.90, 25),
('Teclado Redragon Kumara', 2, 2, 169.90, 15),
('SSD Kingston 1 TB', 4, 1, 289.90, 12);
GO

INSERT INTO MovimientoInventario
(IdProducto, Tipo, Cantidad, Fecha, Motivo)
VALUES
(1, 'Entrada', 8, GETDATE(), 'Stock inicial'),
(2, 'Entrada', 25, GETDATE(), 'Stock inicial'),
(3, 'Entrada', 15, GETDATE(), 'Stock inicial'),
(4, 'Entrada', 12, GETDATE(), 'Stock inicial');
GO

-- ============================================
-- Verificación
-- ============================================
SELECT * FROM Categoria;
SELECT * FROM Proveedor;
SELECT * FROM Producto;
SELECT * FROM MovimientoInventario;
SELECT * FROM Usuario;
GO
