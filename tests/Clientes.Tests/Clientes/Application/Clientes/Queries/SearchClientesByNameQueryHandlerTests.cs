using Clientes.Application.Clientes.Interfaces;
using Clientes.Application.Clientes.Queries.SearchClientesByName;
using Clientes.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Clientes.Tests.Clientes.Application.Clientes.Queries;

[TestFixture]
public class SearchClientesByNameQueryHandlerTests
{
    private Mock<IClienteReadRepository> _readRepositoryMock = null!;
    private Mock<ILogger<SearchClientesByNameQueryHandler>> _loggerMock = null!;
    private SearchClientesByNameQueryHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _readRepositoryMock = new Mock<IClienteReadRepository>();
        _loggerMock = new Mock<ILogger<SearchClientesByNameQueryHandler>>();
        _handler = new SearchClientesByNameQueryHandler(_readRepositoryMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task Handle_ShouldReturnClientes_WhenRepositoryReturnsMatches()
    {
        // Arrange
        var clientes = new List<Cliente>
        {
            new() { Id = 1, Nombre = "Juan" },
            new() { Id = 2, Nombre = "Juana" }
        };

        _readRepositoryMock
            .Setup(r => r.SearchByNameAsync("Juan", It.IsAny<CancellationToken>()))
            .ReturnsAsync(clientes);

        var query = new SearchClientesByNameQuery("Juan");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(clientes);

        _readRepositoryMock.Verify(r => r.SearchByNameAsync("Juan", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_ShouldReturnEmptyList_WhenRepositoryReturnsEmpty()
    {
        // Arrange
        _readRepositoryMock
            .Setup(r => r.SearchByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Cliente>());

        var query = new SearchClientesByNameQuery("NoMatch");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();

        _readRepositoryMock.Verify(r => r.SearchByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_ShouldLogErrorAndRethrow_WhenUnexpectedExceptionOccurs()
    {
        // Arrange
        var exception = new Exception("unexpected");

        _readRepositoryMock
            .Setup(r => r.SearchByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        var query = new SearchClientesByNameQuery("term");

        // Act
        Func<Task> act = () => _handler.Handle(query, CancellationToken.None);

        // Assert
        var thrown = await act.Should().ThrowAsync<Exception>();
        thrown.Which.Should().Be(exception);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error al buscar clientes")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
