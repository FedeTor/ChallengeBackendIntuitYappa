using Clientes.Application.Clientes.Exceptions;
using Clientes.Application.Clientes.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Clientes.Application.Clientes.Commands.DeleteCliente;

public class DeleteClienteCommandHandler : IRequestHandler<DeleteClienteCommand>
{
    private readonly IClienteWriteRepository _clienteWriteRepository;
    private readonly ILogger<DeleteClienteCommandHandler> _logger;

    public DeleteClienteCommandHandler(IClienteWriteRepository clienteWriteRepository, ILogger<DeleteClienteCommandHandler> logger)
    {
        _clienteWriteRepository = clienteWriteRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(DeleteClienteCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Eliminando cliente {Id}", request.Id);

        try
        {
            await _clienteWriteRepository.DeleteAsync(request.Id, cancellationToken);
            _logger.LogInformation("Cliente {Id} eliminado correctamente", request.Id);
            return Unit.Value;
        }
        catch (ClienteNotFoundException)
        {
            _logger.LogWarning("No se encontró el cliente {Id} para eliminar", request.Id);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al eliminar el cliente {Id}", request.Id);
            throw;
        }
    }
}
