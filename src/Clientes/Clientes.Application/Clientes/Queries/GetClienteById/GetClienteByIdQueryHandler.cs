using Clientes.Application.Clientes.Interfaces;
using Clientes.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Clientes.Application.Clientes.Queries.GetClienteById;

public class GetClienteByIdQueryHandler : IRequestHandler<GetClienteByIdQuery, Cliente?>
{
    private readonly IClienteReadRepository _clienteReadRepository;
    private readonly ILogger<GetClienteByIdQueryHandler> _logger;

    public GetClienteByIdQueryHandler(IClienteReadRepository clienteReadRepository, ILogger<GetClienteByIdQueryHandler> logger)
    {
        _clienteReadRepository = clienteReadRepository;
        _logger = logger;
    }

    public async Task<Cliente?> Handle(GetClienteByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Buscando cliente con Id {Id}", request.Id);

        try
        {
            return await _clienteReadRepository.GetByIdAsync(request.Id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el cliente con Id {Id}", request.Id);
            throw;
        }
    }
}
