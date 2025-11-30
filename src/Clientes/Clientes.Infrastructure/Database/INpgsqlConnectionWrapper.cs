using System.Data;
using Npgsql;

namespace Clientes.Infrastructure.Database;

public interface INpgsqlConnectionWrapper : IAsyncDisposable
{
    Task OpenAsync(CancellationToken cancellationToken);
    Task<INpgsqlTransactionWrapper> BeginTransactionAsync(CancellationToken cancellationToken);
    INpgsqlCommandWrapper CreateCommand(string commandText, INpgsqlTransactionWrapper? transaction = null);
}

internal class NpgsqlConnectionWrapper : INpgsqlConnectionWrapper
{
    private readonly NpgsqlConnection _connection;

    public NpgsqlConnectionWrapper(NpgsqlConnection connection)
    {
        _connection = connection;
    }

    public Task OpenAsync(CancellationToken cancellationToken)
    {
        return _connection.OpenAsync(cancellationToken);
    }

    public async Task<INpgsqlTransactionWrapper> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        var transaction = await _connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
        return new NpgsqlTransactionWrapper(transaction);
    }

    public INpgsqlCommandWrapper CreateCommand(string commandText, INpgsqlTransactionWrapper? transaction = null)
    {
        var npgsqlTransaction = (transaction as NpgsqlTransactionWrapper)?.Transaction;
        return new NpgsqlCommandWrapper(new NpgsqlCommand(commandText, _connection, npgsqlTransaction));
    }

    public ValueTask DisposeAsync()
    {
        return _connection.DisposeAsync();
    }
}
