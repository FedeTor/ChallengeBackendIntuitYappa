using Clientes.Domain.Entities;
using MediatR;

namespace Clientes.Application.Clientes.Queries.GetClienteById;

public record GetClienteByIdQuery(int Id) : IRequest<Cliente?>;
