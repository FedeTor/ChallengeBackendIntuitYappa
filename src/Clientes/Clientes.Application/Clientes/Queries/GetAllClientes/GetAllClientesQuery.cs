using Clientes.Domain.Entities;
using MediatR;

namespace Clientes.Application.Clientes.Queries.GetAllClientes;

public record GetAllClientesQuery() : IRequest<IReadOnlyList<Cliente>>;
