using MediatR;

namespace Clientes.Application.Clientes.Commands.CreateCliente;

public record CreateClienteCommand(
    string Nombre,
    string Apellido,
    string RazonSocial,
    string Cuit,
    DateOnly FechaNacimiento,
    string TelefonoCelular,
    string Email
) : IRequest<Domain.Entities.Cliente>;
