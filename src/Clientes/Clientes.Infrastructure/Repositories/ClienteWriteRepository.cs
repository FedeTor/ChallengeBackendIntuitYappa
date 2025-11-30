using System.Data;
using Clientes.Application.Clientes.Exceptions;
using Clientes.Application.Clientes.Interfaces;
using Clientes.Domain.Entities;
using Clientes.Infrastructure.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using NpgsqlTypes;

namespace Clientes.Infrastructure.Repositories;

public class ClienteWriteRepository : IClienteWriteRepository
{
    private readonly string? _connectionString;
    private readonly INpgsqlConnectionFactory _connectionFactory;
    private readonly ILogger<ClienteWriteRepository> _logger;

    public ClienteWriteRepository(
        IConfiguration configuration,
        ILogger<ClienteWriteRepository> logger,
        INpgsqlConnectionFactory connectionFactory)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        _logger = logger;
        _connectionFactory = connectionFactory;
    }

    public async Task<Cliente> InsertAsync(
        string nombre,
        string apellido,
        string razonSocial,
        string cuit,
        DateOnly fechaNacimiento,
        string telefonoCelular,
        string email,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
        }

        try
        {
            await using var connection = _connectionFactory.CreateConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using (var command = connection.CreateCommand("sp_clientes_insert"))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.AddParameter(new NpgsqlParameter("p_nombre", nombre));
                command.AddParameter(new NpgsqlParameter("p_apellido", apellido));
                command.AddParameter(new NpgsqlParameter("p_razon_social", razonSocial));
                command.AddParameter(new NpgsqlParameter("p_cuit", cuit));
                command.AddParameter(new NpgsqlParameter("p_fecha_nacimiento", fechaNacimiento));
                command.AddParameter(new NpgsqlParameter("p_telefono_celular", telefonoCelular));
                command.AddParameter(new NpgsqlParameter("p_email", email));

                var idParameter = new NpgsqlParameter("p_id", NpgsqlDbType.Integer)
                {
                    Direction = ParameterDirection.Output
                };
                command.AddParameter(idParameter);

                await command.ExecuteNonQueryAsync(cancellationToken);

                if (idParameter.Value is DBNull)
                {
                    throw new InvalidOperationException("El procedimiento almacenado no devolvió el id del nuevo cliente.");
                }

                var newId = Convert.ToInt32(idParameter.Value);

                return await GetByIdAsync(connection, newId, cancellationToken);
            }
        }
        catch (PostgresException ex)
        {
            _logger.LogWarning(ex, "Error de base de datos al insertar cliente {Cuit}", cuit);

            if (ex.Message.Contains("Ya existe un cliente con el CUIT", StringComparison.InvariantCultureIgnoreCase) ||
                ex.Message.Contains("Ya existe un cliente con el email", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ClienteConflictException(ex.Message);
            }

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al insertar cliente {Cuit}", cuit);
            throw;
        }
    }

    public async Task<Cliente> UpdateAsync(
        int id,
        string nombre,
        string apellido,
        string razonSocial,
        string cuit,
        DateOnly fechaNacimiento,
        string telefonoCelular,
        string email,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
        }

        try
        {
            await using var connection = _connectionFactory.CreateConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using (var command = connection.CreateCommand("sp_clientes_update"))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.AddParameter(new NpgsqlParameter("p_id", id));
                command.AddParameter(new NpgsqlParameter("p_nombre", nombre));
                command.AddParameter(new NpgsqlParameter("p_apellido", apellido));
                command.AddParameter(new NpgsqlParameter("p_razon_social", razonSocial));
                command.AddParameter(new NpgsqlParameter("p_cuit", cuit));
                command.AddParameter(new NpgsqlParameter("p_fecha_nacimiento", fechaNacimiento));
                command.AddParameter(new NpgsqlParameter("p_telefono_celular", telefonoCelular));
                command.AddParameter(new NpgsqlParameter("p_email", email));

                var rowsParameter = new NpgsqlParameter("p_rows_affected", NpgsqlDbType.Integer)
                {
                    Direction = ParameterDirection.Output
                };
                command.AddParameter(rowsParameter);

                await command.ExecuteNonQueryAsync(cancellationToken);

                if (rowsParameter.Value is DBNull || Convert.ToInt32(rowsParameter.Value) == 0)
                {
                    throw new ClienteNotFoundException($"No existe un cliente con el id {id}");
                }
            }

            return await GetByIdAsync(connection, id, cancellationToken);
        }
        catch (PostgresException ex)
        {
            _logger.LogWarning(ex, "Error de base de datos al actualizar cliente {Id}", id);

            if (ex.Message.Contains("Ya existe un cliente con el CUIT", StringComparison.InvariantCultureIgnoreCase) ||
                ex.Message.Contains("Ya existe un cliente con el email", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ClienteConflictException(ex.Message);
            }

            if (ex.Message.Contains("No existe un cliente con el id", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ClienteNotFoundException(ex.Message);
            }

            throw;
        }
        catch (ClienteNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al actualizar cliente {Id}", id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
        }

        try
        {
            await using var connection = _connectionFactory.CreateConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = connection.CreateCommand("sp_clientes_delete");
            command.CommandType = CommandType.StoredProcedure;

            command.AddParameter(new NpgsqlParameter("p_id", id));

            var rowsParameter = new NpgsqlParameter("p_rows_affected", NpgsqlDbType.Integer)
            {
                Direction = ParameterDirection.Output
            };
            command.AddParameter(rowsParameter);

            await command.ExecuteNonQueryAsync(cancellationToken);

            if (rowsParameter.Value is DBNull || Convert.ToInt32(rowsParameter.Value) == 0)
            {
                throw new ClienteNotFoundException($"No existe un cliente con el id {id}");
            }
        }
        catch (PostgresException ex)
        {
            _logger.LogWarning(ex, "Error de base de datos al eliminar cliente {Id}", id);

            if (ex.Message.Contains("No existe un cliente con el id", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ClienteNotFoundException(ex.Message);
            }

            throw;
        }
        catch (ClienteNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al eliminar cliente {Id}", id);
            throw;
        }
    }

    private static async Task<Cliente> GetByIdAsync(INpgsqlConnectionWrapper connection, int id, CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand("sp_clientes_get_by_id");
        command.CommandType = CommandType.StoredProcedure;

        command.AddParameter(new NpgsqlParameter("p_id", id));

        var outputParameters = new List<NpgsqlParameter>
        {
            new("o_id", NpgsqlDbType.Integer) { Direction = ParameterDirection.Output },
            new("o_nombre", NpgsqlDbType.Varchar) { Direction = ParameterDirection.Output },
            new("o_apellido", NpgsqlDbType.Varchar) { Direction = ParameterDirection.Output },
            new("o_razon_social", NpgsqlDbType.Varchar) { Direction = ParameterDirection.Output },
            new("o_cuit", NpgsqlDbType.Varchar) { Direction = ParameterDirection.Output },
            new("o_fecha_nacimiento", NpgsqlDbType.Date) { Direction = ParameterDirection.Output },
            new("o_telefono_celular", NpgsqlDbType.Varchar) { Direction = ParameterDirection.Output },
            new("o_email", NpgsqlDbType.Varchar) { Direction = ParameterDirection.Output },
            new("o_fecha_creacion", NpgsqlDbType.Timestamp) { Direction = ParameterDirection.Output },
            new("o_fecha_modificacion", NpgsqlDbType.Timestamp) { Direction = ParameterDirection.Output }
        };

        command.AddParameters(outputParameters);

        await command.ExecuteNonQueryAsync(cancellationToken);

        if (command.Parameters["o_id"].Value is DBNull)
        {
            throw new InvalidOperationException($"No se pudo recuperar el cliente con id {id}.");
        }

        return MapCliente(command.Parameters);
    }

    private static Cliente MapCliente(NpgsqlParameterCollection parameters)
    {
        return new Cliente
        {
            Id = Convert.ToInt32(parameters["o_id"].Value),
            Nombre = Convert.ToString(parameters["o_nombre"].Value) ?? string.Empty,
            Apellido = Convert.ToString(parameters["o_apellido"].Value) ?? string.Empty,
            RazonSocial = Convert.ToString(parameters["o_razon_social"].Value) ?? string.Empty,
            Cuit = Convert.ToString(parameters["o_cuit"].Value) ?? string.Empty,
            FechaNacimiento = ToDateOnly(parameters["o_fecha_nacimiento"].Value),
            TelefonoCelular = Convert.ToString(parameters["o_telefono_celular"].Value) ?? string.Empty,
            Email = Convert.ToString(parameters["o_email"].Value) ?? string.Empty,
            FechaCreacion = Convert.ToDateTime(parameters["o_fecha_creacion"].Value),
            FechaModificacion = Convert.ToDateTime(parameters["o_fecha_modificacion"].Value)
        };
    }

    private static DateOnly ToDateOnly(object value)
    {
        return value switch
        {
            DateOnly dateOnly => dateOnly,
            DateTime dateTime => DateOnly.FromDateTime(dateTime),
            _ => DateOnly.FromDateTime(Convert.ToDateTime(value))
        };
    }
}
