using Clientes.Domain.Entities;

namespace Clientes.Application.Clientes.Interfaces;

public interface IClienteReadRepository
{
    Task<IReadOnlyList<Cliente>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Cliente?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Cliente>> SearchByNameAsync(string search, CancellationToken cancellationToken = default);
}
