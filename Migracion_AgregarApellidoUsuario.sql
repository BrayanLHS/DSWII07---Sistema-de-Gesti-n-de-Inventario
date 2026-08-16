-- ============================================
-- Migracion: agrega columna Apellido a la tabla Usuario existente
-- Ejecutar UNA sola vez en la base de datos ya creada (Azure)
-- ============================================

ALTER TABLE Usuario
    ADD Apellido VARCHAR(100) NOT NULL DEFAULT '';
GO

-- Verificacion
SELECT IdUsuario, Nombre, Apellido, Correo, Rol FROM Usuario;
GO
