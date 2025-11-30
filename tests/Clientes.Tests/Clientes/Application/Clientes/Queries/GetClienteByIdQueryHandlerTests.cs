using Clientes.Application.Clientes.Interfaces;
using Clientes.Application.Clientes.Queries.GetClienteById;
using Clientes.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Clientes.Tests.Clientes.Application.Clientes.Queries;

[TestFixture]
public class GetClienteByIdQueryHandlerTests
{
    private Mock<IClienteReadRepository> _readRepositoryMock = null!;
    private Mock<ILogger<GetClienteByIdQueryHandler>> _loggerMock = null!;
    private GetClienteByIdQueryHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _readRepositoryMock = new Mock<IClienteReadRepository>();
        _loggerMock = new Mock<ILogger<GetClienteByIdQueryHandler>>();
        _handler = new GetClienteByIdQueryHandler(_readRepositoryMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task Handle_ShouldReturnCliente_WhenClienteExists()
    {
        // Arrange
        var cliente = new Cliente { Id = 1, Nombre = "Juan" };
        _readRepositoryMock
            .Setup(r => r.GetByIdAsync(cliente.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);

        var query = new GetClienteByIdQuery(cliente.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(cliente);

        _readRepositoryMock.Verify(r => r.GetByIdAsync(cliente.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_ShouldReturnNull_WhenClienteDoesNotExist()
    {
        // Arrange
        _readRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cliente?)null);

        var query = new GetClienteByIdQuery(99);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();

        _readRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_ShouldLogErrorAndRethrow_WhenUnexpectedExceptionOccurs()
    {
        // Arrange
        var exception = new Exception("unexpected");

        _readRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        var query = new GetClienteByIdQuery(1);

        // Act
        Func<Task> act = () => _handler.Handle(query, CancellationToken.None);

        // Assert
        var thrown = await act.Should().ThrowAsync<Exception>();
        thrown.Which.Should().Be(exception);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error al obtener el cliente")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
