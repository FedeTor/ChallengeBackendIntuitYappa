using Clientes.Application.Clientes.Commands.CreateCliente;
using Clientes.Application.Clientes.Exceptions;
using Clientes.Application.Clientes.Interfaces;
using Clientes.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Clientes.Tests.Clientes.Application.Clientes.Commands;

[TestFixture]
public class CreateClienteCommandHandlerTests
{
    private Mock<IClienteWriteRepository> _writeRepositoryMock = null!;
    private Mock<ILogger<CreateClienteCommandHandler>> _loggerMock = null!;
    private CreateClienteCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _writeRepositoryMock = new Mock<IClienteWriteRepository>();
        _loggerMock = new Mock<ILogger<CreateClienteCommandHandler>>();
        _handler = new CreateClienteCommandHandler(_writeRepositoryMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task Handle_ShouldReturnCliente_WhenInsertSucceeds()
    {
        // Arrange
        var command = new CreateClienteCommand(
            "Juan",
            "Perez",
            "JP SRL",
            "20-12345678-9",
            new DateOnly(1990, 1, 1),
            "+5411111111",
            "juan.perez@example.com");

        var expectedCliente = new Cliente
        {
            Id = 1,
            Nombre = command.Nombre,
            Apellido = command.Apellido,
            RazonSocial = command.RazonSocial,
            Cuit = command.Cuit,
            FechaNacimiento = command.FechaNacimiento,
            TelefonoCelular = command.TelefonoCelular,
            Email = command.Email
        };

        _writeRepositoryMock
            .Setup(r => r.InsertAsync(
                command.Nombre,
                command.Apellido,
                command.RazonSocial,
                command.Cuit,
                command.FechaNacimiento,
                command.TelefonoCelular,
                command.Email,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCliente);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedCliente);

        _writeRepositoryMock.Verify(r => r.InsertAsync(
            command.Nombre,
            command.Apellido,
            command.RazonSocial,
            command.Cuit,
            command.FechaNacimiento,
            command.TelefonoCelular,
            command.Email,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_ShouldThrowClienteConflictException_WhenRepositoryThrowsConflict()
    {
        // Arrange
        var command = new CreateClienteCommand(
            "Juan",
            "Perez",
            "JP SRL",
            "20-12345678-9",
            new DateOnly(1990, 1, 1),
            "+5411111111",
            "juan.perez@example.com");

        _writeRepositoryMock
            .Setup(r => r.InsertAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateOnly>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ClienteConflictException("conflict"));

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ClienteConflictException>();

        _writeRepositoryMock.Verify(r => r.InsertAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<DateOnly>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Conflicto al crear cliente")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public async Task Handle_ShouldLogErrorAndRethrow_WhenUnexpectedExceptionOccurs()
    {
        // Arrange
        var command = new CreateClienteCommand(
            "Juan",
            "Perez",
            "JP SRL",
            "20-12345678-9",
            new DateOnly(1990, 1, 1),
            "+5411111111",
            "juan.perez@example.com");

        var exception = new Exception("unexpected");

        _writeRepositoryMock
            .Setup(r => r.InsertAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateOnly>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        var thrown = await act.Should().ThrowAsync<Exception>();
        thrown.Which.Should().Be(exception);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error inesperado al crear cliente")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
