using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Clientes.Domain.Entities;
using Clientes.Infrastructure.Database;
using Clientes.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Npgsql;
using NUnit.Framework;

namespace Clientes.Tests.Clientes.Infrastructure.Repositories;

[TestFixture]
public class ClienteReadRepositoryTests
{
    private IConfiguration _configuration = null!;
    private Mock<INpgsqlConnectionFactory> _connectionFactoryMock = null!;
    private Mock<INpgsqlConnectionWrapper> _connectionMock = null!;
    private Mock<ILogger<ClienteReadRepository>> _loggerMock = null!;

    [SetUp]
    public void SetUp()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=test;User Id=test;Password=test;"
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _connectionFactoryMock = new Mock<INpgsqlConnectionFactory>();
        _connectionMock = new Mock<INpgsqlConnectionWrapper>();
        _loggerMock = new Mock<ILogger<ClienteReadRepository>>();
        _connectionMock.Setup(c => c.OpenAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _connectionMock.Setup(c => c.DisposeAsync()).Returns(ValueTask.CompletedTask);
    }

    [Test]
    public void GetAllAsync_SinConnectionString_LanzaInvalidOperationException()
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>()).Build();
        var repository = new ClienteReadRepository(configuration, _loggerMock.Object, _connectionFactoryMock.Object);

        var action = async () => await repository.GetAllAsync();

        action.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Connection string 'DefaultConnection' is not configured.");
    }

    [Test]
    public async Task GetByIdAsync_WhenClienteExists_ReturnsClienteCorrectamenteMapeado()
    {
        var commandMock = CreateCommandMock();
        var parameters = commandMock.Object.Parameters;

        _connectionMock
            .Setup(c => c.CreateCommand("sp_clientes_get_by_id", null))
            .Returns(commandMock.Object);

        commandMock
            .Setup(c => c.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()))
            .Callback(() =>
            {
                parameters["o_id"].Value = 5;
                parameters["o_nombre"].Value = "Ana";
                parameters["o_apellido"].Value = "Gomez";
                parameters["o_razon_social"].Value = "AG SAS";
                parameters["o_cuit"].Value = "20-12345678-9";
                parameters["o_fecha_nacimiento"].Value = new System.DateTime(1990, 6, 15);
                parameters["o_telefono_celular"].Value = "+5491122334455";
                parameters["o_email"].Value = "ana@example.com";
                parameters["o_fecha_creacion"].Value = new System.DateTime(2024, 1, 1);
                parameters["o_fecha_modificacion"].Value = new System.DateTime(2024, 2, 1);
            })
            .ReturnsAsync(1);

        _connectionFactoryMock
            .Setup(f => f.CreateConnection(It.IsAny<string>()))
            .Returns(_connectionMock.Object);

        var repository = new ClienteReadRepository(_configuration, NullLogger<ClienteReadRepository>.Instance, _connectionFactoryMock.Object);

        var cliente = await repository.GetByIdAsync(5);

        cliente.Should().NotBeNull();
        cliente!.Id.Should().Be(5);
        cliente.Nombre.Should().Be("Ana");
        cliente.Apellido.Should().Be("Gomez");
        cliente.RazonSocial.Should().Be("AG SAS");
        cliente.Cuit.Should().Be("20-12345678-9");
        cliente.FechaNacimiento.Should().Be(new DateOnly(1990, 6, 15));
        cliente.TelefonoCelular.Should().Be("+5491122334455");
        cliente.Email.Should().Be("ana@example.com");
        cliente.FechaCreacion.Should().Be(new System.DateTime(2024, 1, 1));
        cliente.FechaModificacion.Should().Be(new System.DateTime(2024, 2, 1));
    }

    [Test]
    public async Task GetByIdAsync_WhenClienteNoExiste_RetornaNull()
    {
        var commandMock = CreateCommandMock();

        _connectionMock
            .Setup(c => c.CreateCommand("sp_clientes_get_by_id", null))
            .Returns(commandMock.Object);

        commandMock
            .Setup(c => c.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()))
            .Callback(() => commandMock.Object.Parameters["o_id"].Value = DBNull.Value)
            .ReturnsAsync(0);

        _connectionFactoryMock
            .Setup(f => f.CreateConnection(It.IsAny<string>()))
            .Returns(_connectionMock.Object);

        var repository = new ClienteReadRepository(_configuration, NullLogger<ClienteReadRepository>.Instance, _connectionFactoryMock.Object);

        var cliente = await repository.GetByIdAsync(99);

        cliente.Should().BeNull();
    }

    [Test]
    public void GetByIdAsync_CuandoPostgresLanzaError_PropagaLaExcepcion()
    {
        var commandMock = CreateCommandMock();

        _connectionMock
            .Setup(c => c.CreateCommand("sp_clientes_get_by_id", null))
            .Returns(commandMock.Object);

        var postgresException = new PostgresException(
            messageText: "Error inesperado",
            severity: "ERROR",
            invariantSeverity: "ERROR",
            sqlState: "99999",
            detail: string.Empty,
            hint: string.Empty,
            position: 0,
            internalPosition: 0,
            internalQuery: string.Empty,
            where: string.Empty,
            schemaName: string.Empty,
            tableName: string.Empty,
            columnName: string.Empty,
            dataTypeName: string.Empty,
            constraintName: string.Empty,
            file: string.Empty,
            line: "0",
            routine: string.Empty);

        commandMock
            .Setup(c => c.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(postgresException);

        _connectionFactoryMock
            .Setup(f => f.CreateConnection(It.IsAny<string>()))
            .Returns(_connectionMock.Object);

        var repository = new ClienteReadRepository(_configuration, NullLogger<ClienteReadRepository>.Instance, _connectionFactoryMock.Object);

        var action = async () => await repository.GetByIdAsync(1);

        action.Should().ThrowAsync<PostgresException>();
    }

    [Test]
    public async Task GetByIdAsync_CuandoPostgresIndicaNoEncontrado_RegresaNullYLoggeaWarning()
    {
        var commandMock = CreateCommandMock();

        _connectionMock
            .Setup(c => c.CreateCommand("sp_clientes_get_by_id", null))
            .Returns(commandMock.Object);

        var postgresException = new PostgresException(
            messageText: "No se encontró un cliente con el id 50",
            severity: "ERROR",
            invariantSeverity: "ERROR",
            sqlState: "99999",
            detail: string.Empty,
            hint: string.Empty,
            position: 0,
            internalPosition: 0,
            internalQuery: string.Empty,
            where: string.Empty,
            schemaName: string.Empty,
            tableName: string.Empty,
            columnName: string.Empty,
            dataTypeName: string.Empty,
            constraintName: string.Empty,
            file: string.Empty,
            line: "0",
            routine: string.Empty);

        commandMock
            .Setup(c => c.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(postgresException);

        _connectionFactoryMock
            .Setup(f => f.CreateConnection(It.IsAny<string>()))
            .Returns(_connectionMock.Object);

        var repository = new ClienteReadRepository(_configuration, _loggerMock.Object, _connectionFactoryMock.Object);

        var cliente = await repository.GetByIdAsync(50);

        cliente.Should().BeNull();
        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, _) => o.ToString()!.Contains("No se encontró el cliente")),
                postgresException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public async Task GetAllAsync_CuandoExistenClientes_RetornaListaMapeada()
    {
        var transactionMock = new Mock<INpgsqlTransactionWrapper>();
        var getAllCommandMock = CreateCommandMock();
        var fetchCommandMock = CreateCommandMockWithReader();
        var readerMock = CreateReaderWithClientes();

        fetchCommandMock
            .Setup(c => c.ExecuteReaderAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(readerMock);

        var commandQueue = new Queue<INpgsqlCommandWrapper>(new[] { getAllCommandMock.Object, fetchCommandMock.Object });

        _connectionMock
            .Setup(c => c.CreateCommand(It.IsAny<string>(), It.IsAny<INpgsqlTransactionWrapper>()))
            .Returns(() => commandQueue.Dequeue());
        _connectionMock
            .Setup(c => c.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactionMock.Object);

        _connectionFactoryMock
            .Setup(f => f.CreateConnection(It.IsAny<string>()))
            .Returns(_connectionMock.Object);

        var repository = new ClienteReadRepository(_configuration, _loggerMock.Object, _connectionFactoryMock.Object);

        var clientes = await repository.GetAllAsync();

        clientes.Should().HaveCount(2);
        getAllCommandMock.Object.Parameters.Should().ContainSingle(p => p.ParameterName == "p_cursor" && (string)p.Value == "cur_clientes_all");
        transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetAllAsync_CuandoNoHayClientes_RetornaListaVacia()
    {
        var transactionMock = new Mock<INpgsqlTransactionWrapper>();
        var getAllCommandMock = CreateCommandMock();
        var fetchCommandMock = CreateCommandMockWithReader();
        var readerMock = CreateEmptyReader();

        fetchCommandMock
            .Setup(c => c.ExecuteReaderAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(readerMock);

        var commandQueue = new Queue<INpgsqlCommandWrapper>(new[] { getAllCommandMock.Object, fetchCommandMock.Object });

        _connectionMock
            .Setup(c => c.CreateCommand(It.IsAny<string>(), It.IsAny<INpgsqlTransactionWrapper>()))
            .Returns(() => commandQueue.Dequeue());
        _connectionMock
            .Setup(c => c.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactionMock.Object);

        _connectionFactoryMock
            .Setup(f => f.CreateConnection(It.IsAny<string>()))
            .Returns(_connectionMock.Object);

        var repository = new ClienteReadRepository(_configuration, _loggerMock.Object, _connectionFactoryMock.Object);

        var clientes = await repository.GetAllAsync();

        clientes.Should().BeEmpty();
        transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void GetAllAsync_CuandoPostgresFalla_LoggeaYPropaga()
    {
        var transactionMock = new Mock<INpgsqlTransactionWrapper>();
        var getAllCommandMock = CreateCommandMock();

        var postgresException = new PostgresException(
            messageText: "Error inesperado",
            severity: "ERROR",
            invariantSeverity: "ERROR",
            sqlState: "99999",
            detail: string.Empty,
            hint: string.Empty,
            position: 0,
            internalPosition: 0,
            internalQuery: string.Empty,
            where: string.Empty,
            schemaName: string.Empty,
            tableName: string.Empty,
            columnName: string.Empty,
            dataTypeName: string.Empty,
            constraintName: string.Empty,
            file: string.Empty,
            line: "0",
            routine: string.Empty);

        getAllCommandMock
            .Setup(c => c.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(postgresException);

        _connectionMock
            .Setup(c => c.CreateCommand(It.IsAny<string>(), It.IsAny<INpgsqlTransactionWrapper>()))
            .Returns(getAllCommandMock.Object);
        _connectionMock
            .Setup(c => c.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactionMock.Object);

        _connectionFactoryMock
            .Setup(f => f.CreateConnection(It.IsAny<string>()))
            .Returns(_connectionMock.Object);

        var repository = new ClienteReadRepository(_configuration, _loggerMock.Object, _connectionFactoryMock.Object);

        var action = async () => await repository.GetAllAsync();

        action.Should().ThrowAsync<PostgresException>();
        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, _) => o.ToString()!.Contains("Error al obtener todos los clientes")),
                postgresException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public void GetAllAsync_CuandoOcurreExcepcionGenerica_LoggeaYPropaga()
    {
        var transactionMock = new Mock<INpgsqlTransactionWrapper>();
        var getAllCommandMock = CreateCommandMock();

        getAllCommandMock
            .Setup(c => c.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("algo salio mal"));

        _connectionMock
            .Setup(c => c.CreateCommand(It.IsAny<string>(), It.IsAny<INpgsqlTransactionWrapper>()))
            .Returns(getAllCommandMock.Object);
        _connectionMock
            .Setup(c => c.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactionMock.Object);

        _connectionFactoryMock
            .Setup(f => f.CreateConnection(It.IsAny<string>()))
            .Returns(_connectionMock.Object);

        var repository = new ClienteReadRepository(_configuration, _loggerMock.Object, _connectionFactoryMock.Object);

        var action = async () => await repository.GetAllAsync();

        action.Should().ThrowAsync<InvalidOperationException>();
        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, _) => o.ToString()!.Contains("Error al obtener todos los clientes")),
                It.IsAny<InvalidOperationException>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public async Task SearchByNameAsync_CuandoHayResultados_RetornaClientes()
    {
        var transactionMock = new Mock<INpgsqlTransactionWrapper>();
        var searchCommandMock = CreateCommandMock();
        var fetchCommandMock = CreateCommandMockWithReader();
        var readerMock = CreateReaderWithClientes();

        fetchCommandMock
            .Setup(c => c.ExecuteReaderAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(readerMock);

        var commandQueue = new Queue<INpgsqlCommandWrapper>(new[] { searchCommandMock.Object, fetchCommandMock.Object });

        _connectionMock
            .Setup(c => c.CreateCommand(It.IsAny<string>(), It.IsAny<INpgsqlTransactionWrapper>()))
            .Returns(() => commandQueue.Dequeue());
        _connectionMock
            .Setup(c => c.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactionMock.Object);

        _connectionFactoryMock
            .Setup(f => f.CreateConnection(It.IsAny<string>()))
            .Returns(_connectionMock.Object);

        var repository = new ClienteReadRepository(_configuration, _loggerMock.Object, _connectionFactoryMock.Object);

        var clientes = await repository.SearchByNameAsync("an");

        clientes.Should().HaveCount(2);
        searchCommandMock.Object.Parameters.Should().ContainSingle(p => p.ParameterName == "p_search" && (string)p.Value == "an");
        transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task SearchByNameAsync_SinResultados_RetornaListaVacia()
    {
        var transactionMock = new Mock<INpgsqlTransactionWrapper>();
        var searchCommandMock = CreateCommandMock();
        var fetchCommandMock = CreateCommandMockWithReader();
        var readerMock = CreateEmptyReader();

        fetchCommandMock
            .Setup(c => c.ExecuteReaderAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(readerMock);

        var commandQueue = new Queue<INpgsqlCommandWrapper>(new[] { searchCommandMock.Object, fetchCommandMock.Object });

        _connectionMock
            .Setup(c => c.CreateCommand(It.IsAny<string>(), It.IsAny<INpgsqlTransactionWrapper>()))
            .Returns(() => commandQueue.Dequeue());
        _connectionMock
            .Setup(c => c.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactionMock.Object);

        _connectionFactoryMock
            .Setup(f => f.CreateConnection(It.IsAny<string>()))
            .Returns(_connectionMock.Object);

        var repository = new ClienteReadRepository(_configuration, _loggerMock.Object, _connectionFactoryMock.Object);

        var clientes = await repository.SearchByNameAsync("zz");

        clientes.Should().BeEmpty();
        transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void SearchByNameAsync_CuandoPostgresFalla_LoggeaYPropaga()
    {
        var transactionMock = new Mock<INpgsqlTransactionWrapper>();
        var searchCommandMock = CreateCommandMock();
        var postgresException = new PostgresException(
            messageText: "Error db",
            severity: "ERROR",
            invariantSeverity: "ERROR",
            sqlState: "99999",
            detail: string.Empty,
            hint: string.Empty,
            position: 0,
            internalPosition: 0,
            internalQuery: string.Empty,
            where: string.Empty,
            schemaName: string.Empty,
            tableName: string.Empty,
            columnName: string.Empty,
            dataTypeName: string.Empty,
            constraintName: string.Empty,
            file: string.Empty,
            line: "0",
            routine: string.Empty);

        searchCommandMock
            .Setup(c => c.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(postgresException);

        _connectionMock
            .Setup(c => c.CreateCommand(It.IsAny<string>(), It.IsAny<INpgsqlTransactionWrapper>()))
            .Returns(searchCommandMock.Object);
        _connectionMock
            .Setup(c => c.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactionMock.Object);

        _connectionFactoryMock
            .Setup(f => f.CreateConnection(It.IsAny<string>()))
            .Returns(_connectionMock.Object);

        var repository = new ClienteReadRepository(_configuration, _loggerMock.Object, _connectionFactoryMock.Object);

        var action = async () => await repository.SearchByNameAsync("ana");

        action.Should().ThrowAsync<PostgresException>();
        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, _) => o.ToString()!.Contains("Error al buscar clientes por nombre")),
                postgresException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public void SearchByNameAsync_CuandoExcepcionGenerica_LoggeaYPropaga()
    {
        var transactionMock = new Mock<INpgsqlTransactionWrapper>();
        var searchCommandMock = CreateCommandMock();

        searchCommandMock
            .Setup(c => c.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("boom"));

        _connectionMock
            .Setup(c => c.CreateCommand(It.IsAny<string>(), It.IsAny<INpgsqlTransactionWrapper>()))
            .Returns(searchCommandMock.Object);
        _connectionMock
            .Setup(c => c.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(transactionMock.Object);

        _connectionFactoryMock
            .Setup(f => f.CreateConnection(It.IsAny<string>()))
            .Returns(_connectionMock.Object);

        var repository = new ClienteReadRepository(_configuration, _loggerMock.Object, _connectionFactoryMock.Object);

        var action = async () => await repository.SearchByNameAsync("ana");

        action.Should().ThrowAsync<Exception>();
        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, _) => o.ToString()!.Contains("Error al buscar clientes por nombre")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public void MapCliente_DesdeDbDataReader_MapeaCorrectamente()
    {
        var reader = new FakeClienteDataReader(new List<Dictionary<string, object>>
        {
            new()
            {
                ["id"] = 3,
                ["nombre"] = "Maria",
                ["apellido"] = "Lopez",
                ["razon_social"] = "ML SRL",
                ["cuit"] = "20-33333333-3",
                ["fecha_nacimiento"] = new DateOnly(1995, 7, 10),
                ["telefono_celular"] = "+5491199988877",
                ["email"] = "maria@example.com",
                ["fecha_creacion"] = new DateTime(2023, 1, 1),
                ["fecha_modificacion"] = new DateTime(2023, 2, 1)
            }
        });

        reader.Read();

        var method = typeof(ClienteReadRepository).GetMethod(
            "MapCliente",
            BindingFlags.NonPublic | BindingFlags.Static,
            binder: null,
            types: new[] { typeof(DbDataReader) },
            modifiers: null);

        method.Should().NotBeNull();

        var result = (Cliente)method!.Invoke(null, new object[] { reader })!;

        result.Id.Should().Be(3);
        result.Nombre.Should().Be("Maria");
        result.Apellido.Should().Be("Lopez");
        result.RazonSocial.Should().Be("ML SRL");
        result.Cuit.Should().Be("20-33333333-3");
        result.FechaNacimiento.Should().Be(new DateOnly(1995, 7, 10));
        result.TelefonoCelular.Should().Be("+5491199988877");
        result.Email.Should().Be("maria@example.com");
        result.FechaCreacion.Should().Be(new DateTime(2023, 1, 1));
        result.FechaModificacion.Should().Be(new DateTime(2023, 2, 1));
    }

    private static Mock<INpgsqlCommandWrapper> CreateCommandMock()
    {
        var commandMock = new Mock<INpgsqlCommandWrapper>();
        var parameters = new NpgsqlCommand().Parameters;
        parameters.AddRange(new[]
        {
            new NpgsqlParameter("o_id", NpgsqlTypes.NpgsqlDbType.Integer) { Direction = System.Data.ParameterDirection.Output },
            new NpgsqlParameter("o_nombre", NpgsqlTypes.NpgsqlDbType.Varchar) { Direction = System.Data.ParameterDirection.Output },
            new NpgsqlParameter("o_apellido", NpgsqlTypes.NpgsqlDbType.Varchar) { Direction = System.Data.ParameterDirection.Output },
            new NpgsqlParameter("o_razon_social", NpgsqlTypes.NpgsqlDbType.Varchar) { Direction = System.Data.ParameterDirection.Output },
            new NpgsqlParameter("o_cuit", NpgsqlTypes.NpgsqlDbType.Varchar) { Direction = System.Data.ParameterDirection.Output },
            new NpgsqlParameter("o_fecha_nacimiento", NpgsqlTypes.NpgsqlDbType.Date) { Direction = System.Data.ParameterDirection.Output },
            new NpgsqlParameter("o_telefono_celular", NpgsqlTypes.NpgsqlDbType.Varchar) { Direction = System.Data.ParameterDirection.Output },
            new NpgsqlParameter("o_email", NpgsqlTypes.NpgsqlDbType.Varchar) { Direction = System.Data.ParameterDirection.Output },
            new NpgsqlParameter("o_fecha_creacion", NpgsqlTypes.NpgsqlDbType.Timestamp) { Direction = System.Data.ParameterDirection.Output },
            new NpgsqlParameter("o_fecha_modificacion", NpgsqlTypes.NpgsqlDbType.Timestamp) { Direction = System.Data.ParameterDirection.Output }
        });

        commandMock.SetupProperty(c => c.CommandType);
        commandMock.SetupGet(c => c.Parameters).Returns(parameters);
        commandMock
            .Setup(c => c.AddParameter(It.IsAny<NpgsqlParameter>()))
            .Callback<NpgsqlParameter>(p => parameters.Add(p));
        commandMock
            .Setup(c => c.AddParameters(It.IsAny<IEnumerable<NpgsqlParameter>>()))
            .Callback<IEnumerable<NpgsqlParameter>>(p =>
            {
                foreach (var parameter in p)
                {
                    parameters.Add(parameter);
                }
            });
        commandMock.Setup(c => c.DisposeAsync()).Returns(ValueTask.CompletedTask);

        return commandMock;
    }

    private static Mock<INpgsqlCommandWrapper> CreateCommandMockWithReader()
    {
        var commandMock = new Mock<INpgsqlCommandWrapper>();
        var parameters = new NpgsqlCommand().Parameters;
        commandMock.SetupProperty(c => c.CommandType);
        commandMock.SetupGet(c => c.Parameters).Returns(parameters);
        commandMock.Setup(c => c.AddParameter(It.IsAny<NpgsqlParameter>())).Callback<NpgsqlParameter>(p => parameters.Add(p));
        commandMock.Setup(c => c.AddParameters(It.IsAny<IEnumerable<NpgsqlParameter>>())).Callback<IEnumerable<NpgsqlParameter>>(p =>
        {
            foreach (var parameter in p)
            {
                parameters.Add(parameter);
            }
        });
        commandMock.Setup(c => c.DisposeAsync()).Returns(ValueTask.CompletedTask);

        return commandMock;
    }

    private static DbDataReader CreateReaderWithClientes()
    {
        var rows = new List<Dictionary<string, object>>
        {
            new()
            {
                ["id"] = 1,
                ["nombre"] = "Ana",
                ["apellido"] = "Gomez",
                ["razon_social"] = "AG SAS",
                ["cuit"] = "20-12345678-9",
                ["fecha_nacimiento"] = new DateOnly(1990, 6, 15),
                ["telefono_celular"] = "+5491122334455",
                ["email"] = "ana@example.com",
                ["fecha_creacion"] = new DateTime(2024, 1, 1),
                ["fecha_modificacion"] = new DateTime(2024, 2, 1)
            },
            new()
            {
                ["id"] = 2,
                ["nombre"] = "Luis",
                ["apellido"] = "Martinez",
                ["razon_social"] = "LM SRL",
                ["cuit"] = "20-22222222-2",
                ["fecha_nacimiento"] = new DateOnly(1985, 12, 1),
                ["telefono_celular"] = "+5491188899988",
                ["email"] = "luis@example.com",
                ["fecha_creacion"] = new DateTime(2024, 3, 1),
                ["fecha_modificacion"] = new DateTime(2024, 3, 2)
            }
        };

        return new FakeClienteDataReader(rows);
    }

    private static DbDataReader CreateEmptyReader()
    {
        return new FakeClienteDataReader(new List<Dictionary<string, object>>());
    }

    private sealed class FakeClienteDataReader : DbDataReader
    {
        private readonly IReadOnlyList<Dictionary<string, object>> _rows;
        private readonly Dictionary<string, int> _ordinals;
        private int _index = -1;

        public FakeClienteDataReader(IReadOnlyList<Dictionary<string, object>> rows)
        {
            _rows = rows;
            _ordinals = rows.FirstOrDefault()?.Keys
                .Select((name, position) => (name, position))
                .ToDictionary(k => k.name, v => v.position, StringComparer.OrdinalIgnoreCase)
                ?? new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        }

        public override bool Read()
        {
            _index++;
            return _index < _rows.Count;
        }

        public override Task<bool> ReadAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Read());
        }

        public override int GetOrdinal(string name)
        {
            if (_ordinals.TryGetValue(name, out var ordinal))
            {
                return ordinal;
            }

            throw new IndexOutOfRangeException($"Columna {name} no encontrada");
        }

        public override string GetString(int ordinal) => (string)GetValue(ordinal);

        public override int GetInt32(int ordinal) => Convert.ToInt32(GetValue(ordinal));

        public override T GetFieldValue<T>(int ordinal) => (T)GetValue(ordinal);

        public override DateTime GetDateTime(int ordinal) => Convert.ToDateTime(GetValue(ordinal));

        public override object GetValue(int ordinal)
        {
            if (_index < 0 || _index >= _rows.Count)
            {
                throw new InvalidOperationException("Lectura fuera de rango");
            }

            return _rows[_index].ElementAt(ordinal).Value;
        }

        public override int FieldCount => _ordinals.Count;

        public override bool HasRows => _rows.Count > 0;

        public override bool IsDBNull(int ordinal) => GetValue(ordinal) is DBNull;

        public override string GetName(int ordinal) => _ordinals.First(p => p.Value == ordinal).Key;

        public override Type GetFieldType(int ordinal) => GetValue(ordinal)?.GetType() ?? typeof(object);

        public override string GetDataTypeName(int ordinal) => GetFieldType(ordinal).Name;

        public override int GetValues(object[] values)
        {
            var count = Math.Min(values.Length, _ordinals.Count);
            for (var i = 0; i < count; i++)
            {
                values[i] = GetValue(i);
            }

            return count;
        }

        public override bool GetBoolean(int ordinal) => Convert.ToBoolean(GetValue(ordinal));

        public override byte GetByte(int ordinal) => Convert.ToByte(GetValue(ordinal));

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            var data = (byte[])GetValue(ordinal);
            var available = Math.Min(length, data.Length - (int)dataOffset);
            Array.Copy(data, dataOffset, buffer, bufferOffset, available);
            return available;
        }

        public override char GetChar(int ordinal) => Convert.ToChar(GetValue(ordinal));

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            var data = GetString(ordinal).ToCharArray();
            var available = Math.Min(length, data.Length - (int)dataOffset);
            Array.Copy(data, dataOffset, buffer, bufferOffset, available);
            return available;
        }

        public override Guid GetGuid(int ordinal) => (Guid)GetValue(ordinal);

        public override short GetInt16(int ordinal) => Convert.ToInt16(GetValue(ordinal));

        public override long GetInt64(int ordinal) => Convert.ToInt64(GetValue(ordinal));

        public override float GetFloat(int ordinal) => Convert.ToSingle(GetValue(ordinal));

        public override double GetDouble(int ordinal) => Convert.ToDouble(GetValue(ordinal));

        public override decimal GetDecimal(int ordinal) => Convert.ToDecimal(GetValue(ordinal));

        public override int Depth => 0;

        public override bool IsClosed => false;

        public override int RecordsAffected => 0;

        public override bool NextResult() => false;

        public override Task<bool> NextResultAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(false);
        }

        public override IEnumerator GetEnumerator()
        {
            return _rows.GetEnumerator();
        }

        public override object this[int ordinal] => GetValue(ordinal);

        public override object this[string name] => GetValue(GetOrdinal(name));
    }
}
