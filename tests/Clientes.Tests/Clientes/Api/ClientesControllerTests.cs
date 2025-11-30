using Clientes.Api.Controllers;
using Clientes.Api.Models;
using Clientes.Application.Clientes.Commands.CreateCliente;
using Clientes.Application.Clientes.Commands.DeleteCliente;
using Clientes.Application.Clientes.Commands.UpdateCliente;
using Clientes.Application.Clientes.Exceptions;
using Clientes.Application.Clientes.Queries.GetAllClientes;
using Clientes.Application.Clientes.Queries.GetClienteById;
using Clientes.Application.Clientes.Queries.SearchClientesByName;
using Clientes.Domain.Entities;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Clientes.Tests.Clientes.Api;

[TestFixture]
public class ClientesControllerTests
{
    private Mock<IMediator> _mediatorMock = null!;
    private Mock<ILogger<ClientesController>> _loggerMock = null!;
    private ClientesController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<ClientesController>>();
        _controller = new ClientesController(_mediatorMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task GetAll_ShouldReturnOkWithClientes()
    {
        // Arrange
        var clientes = new List<Cliente>
        {
            new() { Id = 1, Nombre = "Ana" },
            new() { Id = 2, Nombre = "Luis" }
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllClientesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(clientes);

        // Act
        var result = await _controller.GetAll(CancellationToken.None);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var response = okResult.Value as IEnumerable<object>;
        response.Should().NotBeNull();
        response!.Count().Should().Be(2);
    }

    [Test]
    public async Task GetAll_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var exception = new Exception("boom");

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllClientesQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        // Act
        var result = await _controller.GetAll(CancellationToken.None);

        // Assert
        var statusResult = result as ObjectResult;
        statusResult.Should().NotBeNull();
        statusResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        statusResult.Value.Should().BeEquivalentTo(new { message = "Ocurrió un error inesperado." });

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error inesperado al obtener todos los clientes")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public async Task GetById_ShouldReturnOk_WhenClienteExists()
    {
        // Arrange
        var cliente = new Cliente { Id = 3, Nombre = "María" };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetClienteByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);

        // Act
        var result = await _controller.GetById(cliente.Id, CancellationToken.None);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var response = okResult.Value as ClienteResponse;
        response.Should().NotBeNull();
        response!.Id.Should().Be(cliente.Id);
        response.Nombre.Should().Be(cliente.Nombre);
    }

    [Test]
    public async Task GetById_ShouldReturnNotFound_WhenClienteIsNull()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetClienteByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cliente?)null);

        // Act
        var result = await _controller.GetById(10, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Test]
    public async Task GetById_ShouldReturnNotFound_WhenClienteNotFoundExceptionThrown()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetClienteByIdQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ClienteNotFoundException("no existe"));

        // Act
        var result = await _controller.GetById(11, CancellationToken.None);

        // Assert
        var notFound = result as NotFoundObjectResult;
        notFound.Should().NotBeNull();
        notFound!.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        notFound.Value.Should().BeEquivalentTo(new { message = "no existe" });

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Cliente 11 no encontrado")),
                It.IsAny<ClienteNotFoundException>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public async Task GetById_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var exception = new Exception("error inesperado");

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetClienteByIdQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        // Act
        var result = await _controller.GetById(12, CancellationToken.None);

        // Assert
        var statusResult = result as ObjectResult;
        statusResult.Should().NotBeNull();
        statusResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        statusResult.Value.Should().BeEquivalentTo(new { message = "Ocurrió un error inesperado." });

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error inesperado al obtener cliente")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public async Task Search_ShouldReturnBadRequest_WhenTermIsEmpty()
    {
        // Act
        var result = await _controller.Search(string.Empty, CancellationToken.None);

        // Assert
        var badRequest = result as ObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        badRequest.Value.Should().BeEquivalentTo(new { message = "El parámetro 'term' es requerido." });

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Solicitud inválida para búsqueda")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public async Task Search_ShouldReturnOk_WhenResultsExist()
    {
        // Arrange
        var clientes = new List<Cliente> { new() { Id = 7, Nombre = "Carla" } };

        _mediatorMock
            .Setup(m => m.Send(It.Is<SearchClientesByNameQuery>(q => q.Search == "ca"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(clientes);

        // Act
        var result = await _controller.Search("ca", CancellationToken.None);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var response = okResult.Value as IEnumerable<ClienteResponse>;
        response.Should().NotBeNull();
        response!.Single().Id.Should().Be(7);
        response.Single().Nombre.Should().Be("Carla");
    }

    [Test]
    public async Task Search_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var exception = new Exception("search fail");

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<SearchClientesByNameQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        // Act
        var result = await _controller.Search("term", CancellationToken.None);

        // Assert
        var statusResult = result as ObjectResult;
        statusResult.Should().NotBeNull();
        statusResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        statusResult.Value.Should().BeEquivalentTo(new { message = "Ocurrió un error inesperado." });

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error inesperado al buscar clientes")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public async Task Create_ShouldReturnCreated_WhenMediatorReturnsCliente()
    {
        // Arrange
        var request = new CreateClienteRequest
        {
            Nombre = "Ana",
            Apellido = "García",
            RazonSocial = "Ana SRL",
            Cuit = "20-12345678-9",
            FechaNacimiento = new DateOnly(1990, 1, 1),
            TelefonoCelular = "+5411111111",
            Email = "ana@example.com"
        };

        var createdCliente = new Cliente
        {
            Id = 99,
            Nombre = request.Nombre,
            Apellido = request.Apellido,
            RazonSocial = request.RazonSocial,
            Cuit = request.Cuit,
            FechaNacimiento = request.FechaNacimiento,
            TelefonoCelular = request.TelefonoCelular,
            Email = request.Email
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateClienteCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdCliente);

        // Act
        var result = await _controller.Create(request, CancellationToken.None);

        // Assert
        var createdResult = result as CreatedResult;
        createdResult.Should().NotBeNull();
        createdResult!.StatusCode.Should().Be(StatusCodes.Status201Created);
        createdResult.Location.Should().Be("api/clientes/99");

        var response = createdResult.Value as ClienteResponse;
        response.Should().NotBeNull();
        response!.Id.Should().Be(99);
    }

    [Test]
    public async Task Create_ShouldReturnConflict_WhenClienteConflictExceptionOccurs()
    {
        // Arrange
        var request = new CreateClienteRequest
        {
            Nombre = "Ana",
            Apellido = "García",
            RazonSocial = "Ana SRL",
            Cuit = "20-12345678-9",
            FechaNacimiento = new DateOnly(1990, 1, 1),
            TelefonoCelular = "+5411111111",
            Email = "ana@example.com"
        };

        var conflictException = new ClienteConflictException("conflict");

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateClienteCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(conflictException);

        // Act
        var result = await _controller.Create(request, CancellationToken.None);

        // Assert
        var conflictResult = result as ObjectResult;
        conflictResult.Should().NotBeNull();
        conflictResult!.StatusCode.Should().Be(StatusCodes.Status409Conflict);
        conflictResult.Value.Should().BeEquivalentTo(new { message = conflictException.Message });

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Conflicto al crear cliente")),
                conflictException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public async Task Create_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var request = new CreateClienteRequest
        {
            Nombre = "Ana",
            Apellido = "García",
            RazonSocial = "Ana SRL",
            Cuit = "20-12345678-9",
            FechaNacimiento = new DateOnly(1990, 1, 1),
            TelefonoCelular = "+5411111111",
            Email = "ana@example.com"
        };

        var exception = new Exception("unexpected");

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateClienteCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        // Act
        var result = await _controller.Create(request, CancellationToken.None);

        // Assert
        var statusResult = result as ObjectResult;
        statusResult.Should().NotBeNull();
        statusResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        statusResult.Value.Should().BeEquivalentTo(new { message = "Ocurrió un error inesperado." });

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error inesperado al crear cliente")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public async Task Update_ShouldReturnOk_WhenUpdatedSuccessfully()
    {
        // Arrange
        var request = new UpdateClienteRequest
        {
            Nombre = "Ana",
            Apellido = "García",
            RazonSocial = "Ana SRL",
            Cuit = "20-12345678-9",
            FechaNacimiento = new DateOnly(1990, 1, 1),
            TelefonoCelular = "+5411111111",
            Email = "ana@example.com"
        };

        var updatedCliente = new Cliente { Id = 1, Nombre = request.Nombre };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateClienteCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedCliente);

        // Act
        var result = await _controller.Update(1, request, CancellationToken.None);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var response = okResult.Value as ClienteResponse;
        response.Should().NotBeNull();
        response!.Id.Should().Be(1);
        response.Nombre.Should().Be("Ana");
    }

    [Test]
    public async Task Update_ShouldReturnNotFound_WhenClienteNotFoundExceptionOccurs()
    {
        // Arrange
        var request = new UpdateClienteRequest();
        var exception = new ClienteNotFoundException("no existe");

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateClienteCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        // Act
        var result = await _controller.Update(2, request, CancellationToken.None);

        // Assert
        var notFound = result as ObjectResult;
        notFound.Should().NotBeNull();
        notFound!.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        notFound.Value.Should().BeEquivalentTo(new { message = exception.Message });

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Cliente 2 no encontrado")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public async Task Update_ShouldReturnConflict_WhenClienteConflictExceptionOccurs()
    {
        // Arrange
        var request = new UpdateClienteRequest();
        var exception = new ClienteConflictException("conflict");

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateClienteCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        // Act
        var result = await _controller.Update(3, request, CancellationToken.None);

        // Assert
        var conflict = result as ObjectResult;
        conflict.Should().NotBeNull();
        conflict!.StatusCode.Should().Be(StatusCodes.Status409Conflict);
        conflict.Value.Should().BeEquivalentTo(new { message = exception.Message });

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Conflicto al actualizar cliente")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public async Task Update_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var request = new UpdateClienteRequest();
        var exception = new Exception("unexpected");

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateClienteCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        // Act
        var result = await _controller.Update(4, request, CancellationToken.None);

        // Assert
        var statusResult = result as ObjectResult;
        statusResult.Should().NotBeNull();
        statusResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        statusResult.Value.Should().BeEquivalentTo(new { message = "Ocurrió un error inesperado." });

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error inesperado al actualizar cliente")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public async Task Delete_ShouldReturnNoContent_WhenDeleted()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<DeleteClienteCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        // Act
        var result = await _controller.Delete(5, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Test]
    public async Task Delete_ShouldReturnNotFound_WhenClienteNotFoundExceptionOccurs()
    {
        // Arrange
        var exception = new ClienteNotFoundException("no existe");

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<DeleteClienteCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        // Act
        var result = await _controller.Delete(6, CancellationToken.None);

        // Assert
        var notFound = result as ObjectResult;
        notFound.Should().NotBeNull();
        notFound!.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        notFound.Value.Should().BeEquivalentTo(new { message = exception.Message });

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Cliente 6 no encontrado")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public async Task Delete_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var exception = new Exception("unexpected");

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<DeleteClienteCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        // Act
        var result = await _controller.Delete(7, CancellationToken.None);

        // Assert
        var statusResult = result as ObjectResult;
        statusResult.Should().NotBeNull();
        statusResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        statusResult.Value.Should().BeEquivalentTo(new { message = "Ocurrió un error inesperado." });

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error inesperado al eliminar cliente")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
