-- ============================================
-- Migracion: agrega columna Rol a la tabla Usuario existente
-- Ejecutar UNA sola vez en la base de datos ya creada (Azure o local)
-- ============================================

ALTER TABLE Usuario
    ADD Rol VARCHAR(20) NOT NULL DEFAULT 'Usuario';
GO

ALTER TABLE Usuario
    ADD CONSTRAINT CK_Usuario_Rol CHECK (Rol IN ('Admin', 'Usuario'));
GO

-- ============================================
-- Promover tu propia cuenta a Admin
-- Reemplaza el correo por el que usaste al registrarte
-- ============================================
UPDATE Usuario
SET Rol = 'Admin'
WHERE Correo = 'REEMPLAZA_TU_CORREO_AQUI';
GO

-- Verificacion
SELECT IdUsuario, Nombre, Correo, Rol FROM Usuario;
GO
