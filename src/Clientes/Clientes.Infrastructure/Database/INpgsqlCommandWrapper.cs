using System.Data;
using System.Data.Common;
using System.Linq;
using Npgsql;

namespace Clientes.Infrastructure.Database;

public interface INpgsqlCommandWrapper : IAsyncDisposable
{
    CommandType CommandType { get; set; }
    NpgsqlParameterCollection Parameters { get; }
    void AddParameter(NpgsqlParameter parameter);
    void AddParameters(IEnumerable<NpgsqlParameter> parameters);
    Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken);
    Task<DbDataReader> ExecuteReaderAsync(CancellationToken cancellationToken);
}

internal class NpgsqlCommandWrapper : INpgsqlCommandWrapper
{
    private readonly NpgsqlCommand _command;

    public NpgsqlCommandWrapper(NpgsqlCommand command)
    {
        _command = command;
    }

    public CommandType CommandType
    {
        get => _command.CommandType;
        set => _command.CommandType = value;
    }

    public NpgsqlParameterCollection Parameters => _command.Parameters;

    public void AddParameter(NpgsqlParameter parameter)
    {
        _command.Parameters.Add(parameter);
    }

    public void AddParameters(IEnumerable<NpgsqlParameter> parameters)
    {
        _command.Parameters.AddRange(parameters.ToArray());
    }

    public Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
    {
        return _command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<DbDataReader> ExecuteReaderAsync(CancellationToken cancellationToken)
    {
        return await _command.ExecuteReaderAsync(cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        return _command.DisposeAsync();
    }
}
