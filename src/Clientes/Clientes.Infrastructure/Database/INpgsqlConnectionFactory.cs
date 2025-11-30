using Npgsql;

namespace Clientes.Infrastructure.Database;

public interface INpgsqlConnectionFactory
{
    INpgsqlConnectionWrapper CreateConnection(string connectionString);
}

public class NpgsqlConnectionFactory : INpgsqlConnectionFactory
{
    public INpgsqlConnectionWrapper CreateConnection(string connectionString)
    {
        return new NpgsqlConnectionWrapper(new NpgsqlConnection(connectionString));
    }
}
