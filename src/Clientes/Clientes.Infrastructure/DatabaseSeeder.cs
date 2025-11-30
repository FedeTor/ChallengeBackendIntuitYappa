using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Clientes.Infrastructure
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken = default)
        {
            var loggerFactory = services.GetService<ILoggerFactory>();
            var logger = loggerFactory?.CreateLogger("DatabaseSeeder");
            var configuration = services.GetService<IConfiguration>();

            if (configuration == null)
            {
                logger?.LogWarning("Configuration service is not available. Skipping database seeding.");
                return;
            }

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                logger?.LogWarning("Connection string 'DefaultConnection' is not configured. Skipping database seeding.");
                return;
            }

            try
            {
                await DatabaseInitializer.EnsureCreatedAsync(connectionString, logger, cancellationToken);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }
    }

    internal static class DatabaseInitializer
    {
        private const string CreateTableSql = @"CREATE TABLE IF NOT EXISTS clientes (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    apellido VARCHAR(100) NOT NULL,
    razon_social VARCHAR(150) NOT NULL,
    cuit VARCHAR(20) NOT NULL UNIQUE,
    fecha_nacimiento DATE NOT NULL,
    telefono_celular VARCHAR(30) NOT NULL,
    email VARCHAR(150) NOT NULL UNIQUE,
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fecha_modificacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);";

        private const string InsertSql = @"INSERT INTO clientes (nombre, apellido, razon_social, cuit, fecha_nacimiento, telefono_celular, email) VALUES
('Juan', 'Pérez', 'JP Servicios SRL', '20-12345678-9', '1985-06-15', '1165874210', 'juan.perez@example.com'),
('María', 'Gómez', 'MG Soluciones', '27-23456789-0', '1990-09-21', '1165874221', 'maria.gomez@example.com'),
('Carlos', 'López', 'CL Construcciones', '23-34567890-1', '1978-01-10', '1165874332', 'carlos.lopez@example.com'),
('Lucía', 'Martínez', 'LM Consultora', '27-45678901-2', '1992-03-05', '1165874443', 'lucia.martinez@example.com'),
('Diego', 'Fernández', 'DF Diseño', '20-56789012-3', '1988-11-22', '1165874554', 'diego.fernandez@example.com');";

        private const string CreateProcedureInsertSql = @"CREATE OR REPLACE PROCEDURE sp_clientes_insert(
    IN  p_nombre           VARCHAR(100),
    IN  p_apellido         VARCHAR(100),
    IN  p_razon_social     VARCHAR(150),
    IN  p_cuit             VARCHAR(20),
    IN  p_fecha_nacimiento DATE,
    IN  p_telefono_celular VARCHAR(30),
    IN  p_email            VARCHAR(150),
    OUT p_id               INTEGER
)
LANGUAGE plpgsql
AS $$
BEGIN
    IF p_nombre IS NULL OR TRIM(p_nombre) = '' THEN
        RAISE EXCEPTION 'El nombre es obligatorio';
    END IF;

    IF p_apellido IS NULL OR TRIM(p_apellido) = '' THEN
        RAISE EXCEPTION 'El apellido es obligatorio';
    END IF;

    IF p_razon_social IS NULL OR TRIM(p_razon_social) = '' THEN
        RAISE EXCEPTION 'La razón social es obligatoria';
    END IF;

    IF p_cuit IS NULL OR TRIM(p_cuit) = '' THEN
        RAISE EXCEPTION 'El CUIT es obligatorio';
    END IF;

    IF p_fecha_nacimiento IS NULL THEN
        RAISE EXCEPTION 'La fecha de nacimiento es obligatoria';
    END IF;

    IF p_telefono_celular IS NULL OR TRIM(p_telefono_celular) = '' THEN
        RAISE EXCEPTION 'El teléfono celular es obligatorio';
    END IF;

    IF p_email IS NULL OR TRIM(p_email) = '' THEN
        RAISE EXCEPTION 'El email es obligatorio';
    END IF;

    IF p_fecha_nacimiento > CURRENT_DATE THEN
        RAISE EXCEPTION 'La fecha de nacimiento no puede ser futura';
    END IF;

    IF p_cuit !~ '^[0-9]{2}-[0-9]{8}-[0-9]$' THEN
        RAISE EXCEPTION 'El formato de CUIT no es válido';
    END IF;

    IF p_email !~ '^[^@]+@[^@]+\.[^@]+$' THEN
        RAISE EXCEPTION 'El formato de email no es válido';
    END IF;

    IF EXISTS (SELECT 1 FROM clientes WHERE cuit = p_cuit) THEN
        RAISE EXCEPTION 'Ya existe un cliente con el CUIT %', p_cuit;
    END IF;

    IF EXISTS (SELECT 1 FROM clientes WHERE email = p_email) THEN
        RAISE EXCEPTION 'Ya existe un cliente con el email %', p_email;
    END IF;

    INSERT INTO clientes (nombre, apellido, razon_social, cuit, fecha_nacimiento, telefono_celular, email)
    VALUES (p_nombre, p_apellido, p_razon_social, p_cuit, p_fecha_nacimiento, p_telefono_celular, p_email)
    RETURNING id INTO p_id;
END;
$$;";

        private const string CreateProcedureUpdateSql = @"CREATE OR REPLACE PROCEDURE sp_clientes_update(
    IN  p_id               INTEGER,
    IN  p_nombre           VARCHAR(100),
    IN  p_apellido         VARCHAR(100),
    IN  p_razon_social     VARCHAR(150),
    IN  p_cuit             VARCHAR(20),
    IN  p_fecha_nacimiento DATE,
    IN  p_telefono_celular VARCHAR(30),
    IN  p_email            VARCHAR(150),
    OUT p_rows_affected    INTEGER
)
LANGUAGE plpgsql
AS $$
BEGIN
    IF p_id IS NULL THEN
        RAISE EXCEPTION 'El id es obligatorio para actualizar un cliente';
    END IF;

    IF p_nombre IS NULL OR TRIM(p_nombre) = '' THEN
        RAISE EXCEPTION 'El nombre es obligatorio';
    END IF;

    IF p_apellido IS NULL OR TRIM(p_apellido) = '' THEN
        RAISE EXCEPTION 'El apellido es obligatorio';
    END IF;

    IF p_razon_social IS NULL OR TRIM(p_razon_social) = '' THEN
        RAISE EXCEPTION 'La razón social es obligatoria';
    END IF;

    IF p_cuit IS NULL OR TRIM(p_cuit) = '' THEN
        RAISE EXCEPTION 'El CUIT es obligatorio';
    END IF;

    IF p_fecha_nacimiento IS NULL THEN
        RAISE EXCEPTION 'La fecha de nacimiento es obligatoria';
    END IF;

    IF p_telefono_celular IS NULL OR TRIM(p_telefono_celular) = '' THEN
        RAISE EXCEPTION 'El teléfono celular es obligatorio';
    END IF;

    IF p_email IS NULL OR TRIM(p_email) = '' THEN
        RAISE EXCEPTION 'El email es obligatorio';
    END IF;

    IF p_fecha_nacimiento > CURRENT_DATE THEN
        RAISE EXCEPTION 'La fecha de nacimiento no puede ser futura';
    END IF;

    IF p_cuit !~ '^[0-9]{2}-[0-9]{8}-[0-9]$' THEN
        RAISE EXCEPTION 'El formato de CUIT no es válido';
    END IF;

    IF p_email !~ '^[^@]+@[^@]+\.[^@]+$' THEN
        RAISE EXCEPTION 'El formato de email no es válido';
    END IF;

    IF EXISTS (SELECT 1 FROM clientes WHERE cuit = p_cuit AND id <> p_id) THEN
        RAISE EXCEPTION 'Ya existe un cliente con el CUIT %', p_cuit;
    END IF;

    IF EXISTS (SELECT 1 FROM clientes WHERE email = p_email AND id <> p_id) THEN
        RAISE EXCEPTION 'Ya existe un cliente con el email %', p_email;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM clientes WHERE id = p_id) THEN
        RAISE EXCEPTION 'No existe un cliente con el id %', p_id;
    END IF;

    UPDATE clientes
    SET nombre = p_nombre,
        apellido = p_apellido,
        razon_social = p_razon_social,
        cuit = p_cuit,
        fecha_nacimiento = p_fecha_nacimiento,
        telefono_celular = p_telefono_celular,
        email = p_email,
        fecha_modificacion = CURRENT_TIMESTAMP
    WHERE id = p_id;

    GET DIAGNOSTICS p_rows_affected = ROW_COUNT;
END;
$$;";

        private const string CreateProcedureDeleteSql = @"CREATE OR REPLACE PROCEDURE sp_clientes_delete(
    IN  p_id            INTEGER,
    OUT p_rows_affected INTEGER
)
LANGUAGE plpgsql
AS $$
BEGIN
    IF p_id IS NULL THEN
        RAISE EXCEPTION 'El id es obligatorio para eliminar un cliente';
    END IF;

    DELETE FROM clientes WHERE id = p_id;

    GET DIAGNOSTICS p_rows_affected = ROW_COUNT;

    IF p_rows_affected = 0 THEN
        RAISE EXCEPTION 'No existe un cliente con el id %', p_id;
    END IF;
END;
$$;";

        private const string CreateProcedureGetByIdSql = @"CREATE OR REPLACE PROCEDURE sp_clientes_get_by_id(
    IN  p_id                 INTEGER,
    OUT o_id                 INTEGER,
    OUT o_nombre             VARCHAR(100),
    OUT o_apellido           VARCHAR(100),
    OUT o_razon_social       VARCHAR(150),
    OUT o_cuit               VARCHAR(20),
    OUT o_fecha_nacimiento   DATE,
    OUT o_telefono_celular   VARCHAR(30),
    OUT o_email              VARCHAR(150),
    OUT o_fecha_creacion     TIMESTAMP,
    OUT o_fecha_modificacion TIMESTAMP
)
LANGUAGE plpgsql
AS $$
BEGIN
    IF p_id IS NULL THEN
        RAISE EXCEPTION 'El id es obligatorio para buscar un cliente';
    END IF;

    SELECT id,
           nombre,
           apellido,
           razon_social,
           cuit,
           fecha_nacimiento,
           telefono_celular,
           email,
           fecha_creacion,
           fecha_modificacion
    INTO o_id,
         o_nombre,
         o_apellido,
         o_razon_social,
         o_cuit,
         o_fecha_nacimiento,
         o_telefono_celular,
         o_email,
         o_fecha_creacion,
         o_fecha_modificacion
    FROM clientes
    WHERE id = p_id;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'No se encontró un cliente con el id %', p_id;
    END IF;
END;
$$;";

        private const string CreateProcedureGetAllSql = @"CREATE OR REPLACE PROCEDURE sp_clientes_get_all(INOUT p_cursor REFCURSOR)
LANGUAGE plpgsql
AS $$
BEGIN
    IF p_cursor IS NULL THEN
        p_cursor := 'cur_clientes_all';
    END IF;

    OPEN p_cursor FOR
        SELECT id,
               nombre,
               apellido,
               razon_social,
               cuit,
               fecha_nacimiento,
               telefono_celular,
               email,
               fecha_creacion,
               fecha_modificacion
        FROM clientes
        ORDER BY id;
END;
$$;";

        private const string CreateProcedureSearchByNameSql = @"CREATE OR REPLACE PROCEDURE sp_clientes_search_by_name(
    IN    p_search TEXT,
    INOUT p_cursor REFCURSOR
)
LANGUAGE plpgsql
AS $$
BEGIN
    IF p_cursor IS NULL THEN
        p_cursor := 'cur_clientes_search';
    END IF;

    IF p_search IS NULL OR TRIM(p_search) = '' THEN
        OPEN p_cursor FOR
            SELECT id,
                   nombre,
                   apellido,
                   razon_social,
                   cuit,
                   fecha_nacimiento,
                   telefono_celular,
                   email,
                   fecha_creacion,
                   fecha_modificacion
            FROM clientes
            ORDER BY apellido, nombre;

        RETURN;
    END IF;

    OPEN p_cursor FOR
        SELECT id,
               nombre,
               apellido,
               razon_social,
               cuit,
               fecha_nacimiento,
               telefono_celular,
               email,
               fecha_creacion,
               fecha_modificacion
        FROM clientes
        WHERE LOWER(nombre) LIKE '%' || LOWER(p_search) || '%'
           OR LOWER(apellido) LIKE '%' || LOWER(p_search) || '%'
        ORDER BY apellido, nombre;
END;
$$;";

        public static async Task EnsureCreatedAsync(string connectionString, ILogger? logger, CancellationToken cancellationToken)
        {
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            await using (var createCommand = new NpgsqlCommand(CreateTableSql, connection))
            {
                await createCommand.ExecuteNonQueryAsync(cancellationToken);
            }

            await using (var createInsertProc = new NpgsqlCommand(CreateProcedureInsertSql, connection))
            {
                await createInsertProc.ExecuteNonQueryAsync(cancellationToken);
            }

            await using (var createUpdateProc = new NpgsqlCommand(CreateProcedureUpdateSql, connection))
            {
                await createUpdateProc.ExecuteNonQueryAsync(cancellationToken);
            }

            await using (var createDeleteProc = new NpgsqlCommand(CreateProcedureDeleteSql, connection))
            {
                await createDeleteProc.ExecuteNonQueryAsync(cancellationToken);
            }

            await using (var createGetByIdProc = new NpgsqlCommand(CreateProcedureGetByIdSql, connection))
            {
                await createGetByIdProc.ExecuteNonQueryAsync(cancellationToken);
            }

            await using (var createGetAllProc = new NpgsqlCommand(CreateProcedureGetAllSql, connection))
            {
                await createGetAllProc.ExecuteNonQueryAsync(cancellationToken);
            }

            await using (var createSearchByNameProc = new NpgsqlCommand(CreateProcedureSearchByNameSql, connection))
            {
                await createSearchByNameProc.ExecuteNonQueryAsync(cancellationToken);
            }

            await using (var countCommand = new NpgsqlCommand("SELECT COUNT(*) FROM clientes", connection))
            {
                var result = await countCommand.ExecuteScalarAsync(cancellationToken);
                if (result is long total && total > 0)
                {
                    logger?.LogInformation("Table 'clientes' already contains data. Seeding skipped.");
                    return;
                }
            }

            await using (var insertCommand = new NpgsqlCommand(InsertSql, connection))
            {
                await insertCommand.ExecuteNonQueryAsync(cancellationToken);
            }

            logger?.LogInformation("Database seeding completed successfully.");
        }
    }
}
