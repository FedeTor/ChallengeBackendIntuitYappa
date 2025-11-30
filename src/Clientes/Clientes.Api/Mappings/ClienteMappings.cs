using Clientes.Api.Models;
using Clientes.Domain.Entities;

namespace Clientes.Api.Mappings;

public static class ClienteMappings
{
    public static ClienteResponse ToResponse(this Cliente cliente)
    {
        return new ClienteResponse
        {
            Id = cliente.Id,
            Nombre = cliente.Nombre,
            Apellido = cliente.Apellido,
            RazonSocial = cliente.RazonSocial,
            Cuit = cliente.Cuit,
            FechaNacimiento = cliente.FechaNacimiento,
            TelefonoCelular = cliente.TelefonoCelular,
            Email = cliente.Email,
            FechaCreacion = cliente.FechaCreacion,
            FechaModificacion = cliente.FechaModificacion
        };
    }
}
