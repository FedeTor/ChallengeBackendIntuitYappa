using Npgsql;

namespace Clientes.Infrastructure.Database;

public interface INpgsqlTransactionWrapper : IAsyncDisposable
{
    Task CommitAsync(CancellationToken cancellationToken);
}

internal class NpgsqlTransactionWrapper : INpgsqlTransactionWrapper
{
    private readonly NpgsqlTransaction _transaction;

    public NpgsqlTransactionWrapper(NpgsqlTransaction transaction)
    {
        _transaction = transaction;
    }

    public NpgsqlTransaction Transaction => _transaction;

    public Task CommitAsync(CancellationToken cancellationToken)
    {
        return _transaction.CommitAsync(cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        return _transaction.DisposeAsync();
    }
}
