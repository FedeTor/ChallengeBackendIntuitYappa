using Clientes.Application.Clientes.Commands.DeleteCliente;
using Clientes.Application.Clientes.Exceptions;
using Clientes.Application.Clientes.Interfaces;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Clientes.Tests.Clientes.Application.Clientes.Commands;

[TestFixture]
public class DeleteClienteCommandHandlerTests
{
    private Mock<IClienteWriteRepository> _writeRepositoryMock = null!;
    private Mock<ILogger<DeleteClienteCommandHandler>> _loggerMock = null!;
    private DeleteClienteCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _writeRepositoryMock = new Mock<IClienteWriteRepository>();
        _loggerMock = new Mock<ILogger<DeleteClienteCommandHandler>>();
        _handler = new DeleteClienteCommandHandler(_writeRepositoryMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task Handle_ShouldCompleteWithoutError_WhenDeleteSucceeds()
    {
        // Arrange
        var command = new DeleteClienteCommand(1);

        _writeRepositoryMock
            .Setup(r => r.DeleteAsync(command.Id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);

        _writeRepositoryMock.Verify(r => r.DeleteAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_ShouldThrowClienteNotFoundException_WhenRepositoryThrowsNotFound()
    {
        // Arrange
        var command = new DeleteClienteCommand(99);

        _writeRepositoryMock
            .Setup(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ClienteNotFoundException("not found"));

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ClienteNotFoundException>();

        _writeRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("No se encontró el cliente")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public async Task Handle_ShouldLogErrorAndRethrow_WhenUnexpectedExceptionOccurs()
    {
        // Arrange
        var command = new DeleteClienteCommand(1);
        var exception = new Exception("unexpected");

        _writeRepositoryMock
            .Setup(r => r.DeleteAsync(command.Id, It.IsAny<CancellationToken>()))
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
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error inesperado al eliminar el cliente")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
