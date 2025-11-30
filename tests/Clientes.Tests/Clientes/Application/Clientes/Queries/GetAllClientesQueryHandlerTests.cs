using Clientes.Application.Clientes.Interfaces;
using Clientes.Application.Clientes.Queries.GetAllClientes;
using Clientes.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Clientes.Tests.Clientes.Application.Clientes.Queries;

[TestFixture]
public class GetAllClientesQueryHandlerTests
{
    private Mock<IClienteReadRepository> _readRepositoryMock = null!;
    private Mock<ILogger<GetAllClientesQueryHandler>> _loggerMock = null!;
    private GetAllClientesQueryHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _readRepositoryMock = new Mock<IClienteReadRepository>();
        _loggerMock = new Mock<ILogger<GetAllClientesQueryHandler>>();
        _handler = new GetAllClientesQueryHandler(_readRepositoryMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task Handle_ShouldReturnClientes_WhenRepositoryReturnsData()
    {
        // Arrange
        var clientes = new List<Cliente>
        {
            new() { Id = 1, Nombre = "Juan" },
            new() { Id = 2, Nombre = "Ana" }
        };

        _readRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(clientes);

        var query = new GetAllClientesQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(clientes.Count);
        result.Should().BeEquivalentTo(clientes);

        _readRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_ShouldReturnEmptyList_WhenRepositoryReturnsEmpty()
    {
        // Arrange
        _readRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Cliente>());

        var query = new GetAllClientesQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();

        _readRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_ShouldLogErrorAndRethrow_WhenUnexpectedExceptionOccurs()
    {
        // Arrange
        var exception = new Exception("unexpected");

        _readRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        var query = new GetAllClientesQuery();

        // Act
        Func<Task> act = () => _handler.Handle(query, CancellationToken.None);

        // Assert
        var thrown = await act.Should().ThrowAsync<Exception>();
        thrown.Which.Should().Be(exception);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error al obtener el listado de clientes")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
