using Clientes.Domain.Entities;
using MediatR;

namespace Clientes.Application.Clientes.Queries.SearchClientesByName;

public record SearchClientesByNameQuery(string Search) : IRequest<IReadOnlyList<Cliente>>;
