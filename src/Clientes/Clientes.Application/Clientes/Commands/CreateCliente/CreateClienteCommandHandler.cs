using Clientes.Application.Clientes.Exceptions;
using Clientes.Application.Clientes.Interfaces;
using Clientes.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Clientes.Application.Clientes.Commands.CreateCliente;

public class CreateClienteCommandHandler : IRequestHandler<CreateClienteCommand, Cliente>
{
    private readonly IClienteWriteRepository _clienteWriteRepository;
    private readonly ILogger<CreateClienteCommandHandler> _logger;

    public CreateClienteCommandHandler(
        IClienteWriteRepository clienteWriteRepository,
        ILogger<CreateClienteCommandHandler> logger)
    {
        _clienteWriteRepository = clienteWriteRepository;
        _logger = logger;
    }

    public async Task<Cliente> Handle(CreateClienteCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Procesando alta de cliente {Cuit}", request.Cuit);

        try
        {
            var cliente = await _clienteWriteRepository.InsertAsync(
                request.Nombre,
                request.Apellido,
                request.RazonSocial,
                request.Cuit,
                request.FechaNacimiento,
                request.TelefonoCelular,
                request.Email,
                cancellationToken);

            _logger.LogInformation("Cliente creado con Id {Id}", cliente.Id);

            return cliente;
        }
        catch (ClienteConflictException)
        {
            _logger.LogWarning("Conflicto al crear cliente {Cuit}", request.Cuit);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al crear cliente {Cuit}", request.Cuit);
            throw;
        }
    }
}
