using System.Data;
using System.Data.Common;
using Clientes.Application.Clientes.Interfaces;
using Clientes.Domain.Entities;
using Clientes.Infrastructure.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using NpgsqlTypes;

namespace Clientes.Infrastructure.Repositories;

public class ClienteReadRepository : IClienteReadRepository
{
    private readonly string? _connectionString;
    private readonly INpgsqlConnectionFactory _connectionFactory;
    private readonly ILogger<ClienteReadRepository> _logger;

    public ClienteReadRepository(
        IConfiguration configuration,
        ILogger<ClienteReadRepository> logger,
        INpgsqlConnectionFactory connectionFactory)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        _logger = logger;
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<Cliente>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
        }

        var clientes = new List<Cliente>();

        try
        {
            await using var connection = _connectionFactory.CreateConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
            await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

            await using (var command = connection.CreateCommand("sp_clientes_get_all", transaction))
            {
                command.CommandType = CommandType.StoredProcedure;

                var cursorParameter = new NpgsqlParameter("p_cursor", NpgsqlDbType.Refcursor)
                {
                    Direction = ParameterDirection.InputOutput,
                    Value = "cur_clientes_all"
                };

                command.AddParameter(cursorParameter);

                await command.ExecuteNonQueryAsync(cancellationToken);

                var cursorName = Convert.ToString(cursorParameter.Value) ?? "cur_clientes_all";

                await using var fetchCommand = connection.CreateCommand($"FETCH ALL FROM {cursorName}", transaction);
                await using var reader = await fetchCommand.ExecuteReaderAsync(cancellationToken);

                while (await reader.ReadAsync(cancellationToken))
                {
                    clientes.Add(MapCliente(reader));
                }
            }

            await transaction.CommitAsync(cancellationToken);

            return clientes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los clientes");
            throw;
        }
    }

    public async Task<Cliente?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
        }

        try
        {
            await using var connection = _connectionFactory.CreateConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

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
                return null;
            }

            return MapCliente(command.Parameters);
        }
        catch (PostgresException ex) when (ex.Message.Contains("No se encontró un cliente", StringComparison.InvariantCultureIgnoreCase))
        {
            _logger.LogWarning(ex, "No se encontró el cliente con id {Id}", id);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el cliente con id {Id}", id);
            throw;
        }
    }

    public async Task<IReadOnlyList<Cliente>> SearchByNameAsync(string search, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
        }

        var clientes = new List<Cliente>();

        try
        {
            await using var connection = _connectionFactory.CreateConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
            await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

            await using (var command = connection.CreateCommand("sp_clientes_search_by_name", transaction))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.AddParameter(new NpgsqlParameter("p_search", search));

                var cursorParameter = new NpgsqlParameter("p_cursor", NpgsqlDbType.Refcursor)
                {
                    Direction = ParameterDirection.InputOutput,
                    Value = "cur_clientes_search"
                };

                command.AddParameter(cursorParameter);

                await command.ExecuteNonQueryAsync(cancellationToken);

                var cursorName = Convert.ToString(cursorParameter.Value) ?? "cur_clientes_search";

                await using var fetchCommand = connection.CreateCommand($"FETCH ALL FROM {cursorName}", transaction);
                await using var reader = await fetchCommand.ExecuteReaderAsync(cancellationToken);

                while (await reader.ReadAsync(cancellationToken))
                {
                    clientes.Add(MapCliente(reader));
                }
            }

            await transaction.CommitAsync(cancellationToken);

            return clientes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar clientes por nombre {Search}", search);
            throw;
        }
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

    private static Cliente MapCliente(DbDataReader reader)
    {
        return new Cliente
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            Nombre = reader.GetString(reader.GetOrdinal("nombre")),
            Apellido = reader.GetString(reader.GetOrdinal("apellido")),
            RazonSocial = reader.GetString(reader.GetOrdinal("razon_social")),
            Cuit = reader.GetString(reader.GetOrdinal("cuit")),
            FechaNacimiento = reader.GetFieldValue<DateOnly>(reader.GetOrdinal("fecha_nacimiento")),
            TelefonoCelular = reader.GetString(reader.GetOrdinal("telefono_celular")),
            Email = reader.GetString(reader.GetOrdinal("email")),
            FechaCreacion = reader.GetDateTime(reader.GetOrdinal("fecha_creacion")),
            FechaModificacion = reader.GetDateTime(reader.GetOrdinal("fecha_modificacion"))
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
