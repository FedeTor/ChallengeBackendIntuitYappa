using MediatR;

namespace Clientes.Application.Clientes.Commands.DeleteCliente;

public record DeleteClienteCommand(int Id) : IRequest;
