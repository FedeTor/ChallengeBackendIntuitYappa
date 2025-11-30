using Clientes.Application.Clientes.Interfaces;
using Clientes.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Clientes.Application.Clientes.Queries.GetAllClientes;

public class GetAllClientesQueryHandler : IRequestHandler<GetAllClientesQuery, IReadOnlyList<Cliente>>
{
    private readonly IClienteReadRepository _clienteReadRepository;
    private readonly ILogger<GetAllClientesQueryHandler> _logger;

    public GetAllClientesQueryHandler(IClienteReadRepository clienteReadRepository, ILogger<GetAllClientesQueryHandler> logger)
    {
        _clienteReadRepository = clienteReadRepository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Cliente>> Handle(GetAllClientesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Recuperando listado completo de clientes");

        try
        {
            return await _clienteReadRepository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el listado de clientes");
            throw;
        }
    }
}
