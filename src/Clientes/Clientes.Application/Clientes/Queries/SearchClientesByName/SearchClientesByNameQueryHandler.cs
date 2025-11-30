using Clientes.Application.Clientes.Interfaces;
using Clientes.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Clientes.Application.Clientes.Queries.SearchClientesByName;

public class SearchClientesByNameQueryHandler : IRequestHandler<SearchClientesByNameQuery, IReadOnlyList<Cliente>>
{
    private readonly IClienteReadRepository _clienteReadRepository;
    private readonly ILogger<SearchClientesByNameQueryHandler> _logger;

    public SearchClientesByNameQueryHandler(IClienteReadRepository clienteReadRepository, ILogger<SearchClientesByNameQueryHandler> logger)
    {
        _clienteReadRepository = clienteReadRepository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Cliente>> Handle(SearchClientesByNameQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Buscando clientes por término {Search}", request.Search);

        try
        {
            return await _clienteReadRepository.SearchByNameAsync(request.Search, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar clientes con término {Search}", request.Search);
            throw;
        }
    }
}
