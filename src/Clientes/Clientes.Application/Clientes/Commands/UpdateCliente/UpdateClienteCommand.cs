using MediatR;

namespace Clientes.Application.Clientes.Commands.UpdateCliente;

public record UpdateClienteCommand(
    int Id,
    string Nombre,
    string Apellido,
    string RazonSocial,
    string Cuit,
    DateOnly FechaNacimiento,
    string TelefonoCelular,
    string Email) : IRequest<Domain.Entities.Cliente>;
