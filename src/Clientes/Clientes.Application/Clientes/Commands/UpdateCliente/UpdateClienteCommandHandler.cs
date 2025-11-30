using Clientes.Application.Clientes.Exceptions;
using Clientes.Application.Clientes.Interfaces;
using Clientes.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Clientes.Application.Clientes.Commands.UpdateCliente;

public class UpdateClienteCommandHandler : IRequestHandler<UpdateClienteCommand, Cliente>
{
    private readonly IClienteWriteRepository _clienteWriteRepository;
    private readonly ILogger<UpdateClienteCommandHandler> _logger;

    public UpdateClienteCommandHandler(IClienteWriteRepository clienteWriteRepository, ILogger<UpdateClienteCommandHandler> logger)
    {
        _clienteWriteRepository = clienteWriteRepository;
        _logger = logger;
    }

    public async Task<Cliente> Handle(UpdateClienteCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Actualizando cliente {Id}", request.Id);

        try
        {
            var cliente = await _clienteWriteRepository.UpdateAsync(
                request.Id,
                request.Nombre,
                request.Apellido,
                request.RazonSocial,
                request.Cuit,
                request.FechaNacimiento,
                request.TelefonoCelular,
                request.Email,
                cancellationToken);

            _logger.LogInformation("Cliente {Id} actualizado correctamente", request.Id);

            return cliente;
        }
        catch (ClienteConflictException)
        {
            _logger.LogWarning("Conflicto al actualizar el cliente {Id}", request.Id);
            throw;
        }
        catch (ClienteNotFoundException)
        {
            _logger.LogWarning("No se encontró el cliente {Id} para actualizar", request.Id);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al actualizar el cliente {Id}", request.Id);
            throw;
        }
    }
}
