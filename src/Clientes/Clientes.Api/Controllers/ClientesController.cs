using Clientes.Api.Mappings;
using Clientes.Api.Models;
using Clientes.Application.Clientes.Commands.CreateCliente;
using Clientes.Application.Clientes.Commands.DeleteCliente;
using Clientes.Application.Clientes.Commands.UpdateCliente;
using Clientes.Application.Clientes.Exceptions;
using Clientes.Application.Clientes.Queries.GetAllClientes;
using Clientes.Application.Clientes.Queries.GetClienteById;
using Clientes.Application.Clientes.Queries.SearchClientesByName;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Clientes.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ClientesController> _logger;

    public ClientesController(IMediator mediator, ILogger<ClientesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los clientes.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClienteResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var clientes = await _mediator.Send(new GetAllClientesQuery(), cancellationToken);
            return Ok(clientes.Select(c => c.ToResponse()));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al obtener todos los clientes");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error inesperado." });
        }
    }

    /// <summary>
    /// Obtiene un cliente por su identificador.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var cliente = await _mediator.Send(new GetClienteByIdQuery(id), cancellationToken);

            if (cliente is null)
            {
                return NotFound();
            }

            return Ok(cliente.ToResponse());
        }
        catch (ClienteNotFoundException ex)
        {
            _logger.LogWarning(ex, "Cliente {Id} no encontrado", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al obtener cliente {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error inesperado." });
        }
    }

    /// <summary>
    /// Busca clientes por nombre o apellido.
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<ClienteResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Search([FromQuery] string term, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            _logger.LogWarning("Solicitud inválida para búsqueda de clientes sin término");
            return BadRequest(new { message = "El parámetro 'term' es requerido." });
        }

        try
        {
            var clientes = await _mediator.Send(new SearchClientesByNameQuery(term), cancellationToken);
            return Ok(clientes.Select(c => c.ToResponse()));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al buscar clientes con término {Term}", term);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error inesperado." });
        }
    }

    /// <summary>
    /// Crea un nuevo cliente.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateClienteRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Solicitud inválida para crear cliente: {Errors}", ModelState);
            return ValidationProblem(ModelState);
        }

        var command = new CreateClienteCommand(
            request.Nombre,
            request.Apellido,
            request.RazonSocial,
            request.Cuit,
            request.FechaNacimiento,
            request.TelefonoCelular,
            request.Email);

        try
        {
            var cliente = await _mediator.Send(command, cancellationToken);

            var response = cliente.ToResponse();

            return Created($"api/clientes/{response.Id}", response);
        }
        catch (ClienteConflictException ex)
        {
            _logger.LogWarning(ex, "Conflicto al crear cliente con CUIT {Cuit}", request.Cuit);
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al crear cliente con CUIT {Cuit}", request.Cuit);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error inesperado." });
        }
    } 

    /// <summary>
    /// Actualiza un cliente existente.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateClienteRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Solicitud inválida para actualizar cliente: {Errors}", ModelState);
            return ValidationProblem(ModelState);
        }

        var command = new UpdateClienteCommand(
            id,
            request.Nombre,
            request.Apellido,
            request.RazonSocial,
            request.Cuit,
            request.FechaNacimiento,
            request.TelefonoCelular,
            request.Email);

        try
        {
            var cliente = await _mediator.Send(command, cancellationToken);
            return Ok(cliente.ToResponse());
        }
        catch (ClienteConflictException ex)
        {
            _logger.LogWarning(ex, "Conflicto al actualizar cliente {Id}", id);
            return Conflict(new { message = ex.Message });
        }
        catch (ClienteNotFoundException ex)
        {
            _logger.LogWarning(ex, "Cliente {Id} no encontrado para actualización", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al actualizar cliente {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error inesperado." });
        }
    }

    /// <summary>
    /// Elimina un cliente existente.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            await _mediator.Send(new DeleteClienteCommand(id), cancellationToken);
            return NoContent();
        }
        catch (ClienteNotFoundException ex)
        {
            _logger.LogWarning(ex, "Cliente {Id} no encontrado para eliminación", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al eliminar cliente {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error inesperado." });
        }
    }
}
