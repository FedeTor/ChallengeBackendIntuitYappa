using Clientes.Domain.Entities;

namespace Clientes.Application.Clientes.Interfaces;

public interface IClienteWriteRepository
{
    Task<Cliente> InsertAsync(
        string nombre,
        string apellido,
        string razonSocial,
        string cuit,
        DateOnly fechaNacimiento,
        string telefonoCelular,
        string email,
        CancellationToken cancellationToken = default);

    Task<Cliente> UpdateAsync(
        int id,
        string nombre,
        string apellido,
        string razonSocial,
        string cuit,
        DateOnly fechaNacimiento,
        string telefonoCelular,
        string email,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
