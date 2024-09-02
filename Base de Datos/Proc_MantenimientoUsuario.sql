USE CafeXperienceDB
GO

-- PROCEDIMIENTO PARA MANTENIMIENTO DE USUARIOS Y CONSULTA DE CAFETERIAS
Create PROC sp_MantenimientoUsuarios(
    @Id INT = 0,
    @Nombre VARCHAR(50) = '',
    @Cedula VARCHAR(50) = '',
    @TipoUsuarioId INT = 0,
    @Limite_Credito INT = 0,  
    @Fecha_Registro DATETIME = NULL,  
    @Estado NCHAR(1) = '',
    @Correo VARCHAR(50) = '',
    @Contraseña VARCHAR(50) = '',
    @Opcion INT,
    @Resultado BIT OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validar que @TipoUsuarioId exista en la tabla TipoUsuarios
        IF NOT EXISTS (SELECT 1 FROM TipoUsuarios WHERE Id = @TipoUsuarioId)
        BEGIN
            RAISERROR('El TipoUsuarioId especificado no existe en la tabla TipoUsuarios.', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 1: INSERTAR NUEVO USUARIO
        IF @Opcion = 1
        BEGIN
            INSERT INTO Usuarios (Nombre, Cedula, TipoUsuarioId, Limite_de_Credito, Fecha_Registro, Estado, Correo, Contraseña)
            VALUES (@Nombre, @Cedula, @TipoUsuarioId, @Limite_Credito, ISNULL(@Fecha_Registro, GETDATE()), @Estado, @Correo, @Contraseña);
            
            SET @Resultado = 1;
        END
        
        -- 2: ACTUALIZAR USUARIO EXISTENTE
        ELSE IF @Opcion = 2
        BEGIN
            UPDATE Usuarios
            SET Nombre = @Nombre,
                Cedula = @Cedula,
                TipoUsuarioId = @TipoUsuarioId,
                Limite_de_Credito = @Limite_Credito,  -- Nombre de columna corregido
                Fecha_Registro = ISNULL(@Fecha_Registro, GETDATE()),  -- Nombre de columna corregido si es necesario
                Estado = @Estado,
                Correo = @Correo,
                Contraseña = @Contraseña
            WHERE Id = @Id;

            SET @Resultado = 1;
        END
        
        -- 3: ELIMINAR USUARIO
        ELSE IF @Opcion = 3
        BEGIN
            DELETE FROM Usuarios WHERE Id = @Id;
            SET @Resultado = 1;
        END
        
        -- 4: OBTENER USUARIO POR ID
        ELSE IF @Opcion = 4
        BEGIN
            SELECT * FROM Usuarios WHERE Id = @Id;
            SET @Resultado = 1;
        END

        -- 5: CONSULTAR TODOS LOS CAMPOS DE LA TABLA 'Usuarios' CON FILTROS ESPECÍFICOS
        ELSE IF @Opcion = 5
        BEGIN
            SELECT 
                Id, 
                Nombre, 
				Cedula,
				TipoUsuarioId,
				Correo,
                Estado 
            FROM 
                Usuarios
            WHERE
                (@Id = 0 OR Id = @Id) AND
                (@Nombre = '' + @Nombre + '%') AND
                (@Fecha_Registro IS NULL OR Fecha_registro = @Fecha_Registro) AND 
                (@Estado = '' OR Estado = @Estado);
            
            SET @Resultado = 1;
        END

        -- 6: CONSULTAR USUARIO Y TIPO DE USUARIO POR CORREO Y CONTRASEÑA
        ELSE IF @Opcion = 6
        BEGIN
            SELECT 
                U.Id,
                U.Nombre,
                U.Correo,
                U.Contraseña,
                T.Descripcion AS TipoUsuarioDescripcion
            FROM 
                Usuarios U
            INNER JOIN 
                TipoUsuarios T ON U.TipoUsuarioId = T.Id
            WHERE 
                U.Correo = @Correo 
                AND U.Contraseña = @Contraseña;
                
            SET @Resultado = 1;
        END

        -- SI LA OPCIÓN NO ES VÁLIDA
        ELSE
        BEGIN
            SET @Resultado = 0;
        END

        -- CONFIRMAR TRANSACCIÓN
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        -- EN CASO DE ERROR, REVERTIR TRANSACCIÓN Y DEVOLVER EL MENSAJE DE ERROR
        ROLLBACK TRANSACTION;
        SET @Resultado = 0;

        -- OPCIONAL: DEVOLVER DETALLES DEL ERROR
        DECLARE @ErrorMessage NVARCHAR(4000);
        SET @ErrorMessage = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO
