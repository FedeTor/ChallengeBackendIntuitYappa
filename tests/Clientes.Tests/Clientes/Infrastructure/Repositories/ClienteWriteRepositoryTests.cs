using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Clientes.Application.Clientes.Exceptions;
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
public class ClienteWriteRepositoryTests
{
    private IConfiguration _configuration = null!;
    private Mock<INpgsqlConnectionFactory> _connectionFactoryMock = null!;
    private Mock<INpgsqlConnectionWrapper> _connectionMock = null!;
    private Mock<ILogger<ClienteWriteRepository>> _loggerMock = null!;

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
        _loggerMock = new Mock<ILogger<ClienteWriteRepository>>();
        _connectionMock.Setup(c => c.OpenAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _connectionMock.Setup(c => c.DisposeAsync()).Returns(ValueTask.CompletedTask);
    }

    [Test]
    public async Task InsertAsync_CuandoClienteSeInserta_RetornaClienteMapeado()
    {
        var insertCommandMock = CreateCommandMock();
        var insertParameters = insertCommandMock.Object.Parameters;
        insertCommandMock
            .Setup(c => c.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()))
            .Callback(() => insertParameters["p_id"].Value = 7)
            .ReturnsAsync(1);

        var getCommandMock = CreateCommandMockForGetById();
        var getParameters = getCommandMock.Object.Parameters;
        getCommandMock
            .Setup(c => c.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()))
            .Callback(() =>
            {
                getParameters["o_id"].Value = 7;
                getParameters["o_nombre"].Value = "Luis";
                getParameters["o_apellido"].Value = "Martinez";
                getParameters["o_razon_social"].Value = "LM SRL";
                getParameters["o_cuit"].Value = "20-22222222-2";
                getParameters["o_fecha_nacimiento"].Value = new System.DateTime(1985, 12, 1);
                getParameters["o_telefono_celular"].Value = "+5491188899988";
                getParameters["o_email"].Value = "luis@example.com";
                getParameters["o_fecha_creacion"].Value = new System.DateTime(2024, 3, 1);
                getParameters["o_fecha_modificacion"].Value = new System.DateTime(2024, 3, 2);
            })
            .ReturnsAsync(1);

        var commandQueue = new Queue<INpgsqlCommandWrapper>(new[] { insertCommandMock.Object, getCommandMock.Object });
        _connectionMock
            .Setup(c => c.CreateCommand(It.IsAny<string>(), It.IsAny<INpgsqlTransactionWrapper>()))
            .Returns(() => commandQueue.Dequeue());
        _connectionMock
            .Setup(c => c.CreateCommand(It.Is<string>(s => s == "sp_clientes_get_by_id"), null))
            .Returns(getCommandMock.Object);

        _connectionFactoryMock
            .Setup(f => f.CreateConnection(It.IsAny<string>()))
            .Returns(_connectionMock.Object);

        var repository = new ClienteWriteRepository(_configuration, NullLogger<ClienteWriteRepository>.Instance, _connectionFactoryMock.Object);

        var cliente = await repository.InsertAsync("Luis", "Martinez", "LM SRL", "20-22222222-2", new DateOnly(1985, 12, 1), "+5491188899988", "luis@example.com");

        cliente.Id.Should().Be(7);
        insertParameters.Should().Contain(p => p.ParameterName == "p_nombre" && (string)p.Value == "Luis");
        cliente.FechaNacimiento.Should().Be(new DateOnly(1985, 12, 1));
    }

    [Test]
    public void InsertAsync_CuandoConnectionStringNoExiste_LanzaInvalidOperationException()
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>()).Build();
        var repository = new ClienteWriteRepository(configuration, _loggerMock.Object, _connectionFactoryMock.Object);

        var action = async () => await repository.InsertAsync("n", "a", "r", "c", new DateOnly(2000, 1, 1), "t", "e");

        action.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Connection string 'DefaultConnection' is not configured.");
    }

    [Test]
    public async Task UpdateAsync_CuandoClienteExiste_RetornaClienteActualizado()
    {
        var updateCommandMock = CreateCommandMock();
        var updateParameters = updateCommandMock.Object.Parameters;
        updateCommandMock
            .Setup(c => c.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()))
            .Callback(() => updateParameters["p_rows_affected"].Value = 1)
            .ReturnsAsync(1);

        var getCommandMock = CreateCommandMockForGetById();
        var getParameters = getCommandMock.Object.Parameters;
        getCommandMock
            .Setup(c => c.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()))
            .Callback(() =>
            {
                getParameters["o_id"].Value = 8;
                getParameters["o_nombre"].Value = "Ramon";
                getParameters["o_apellido"].Value = "Diaz";
                getParameters["o_razon_social"].Value = "RD SA";
                getParameters["o_cuit"].Value = "20-33333333-3";
                getParameters["o_fecha_nacimiento"].Value = new System.DateTime(1980, 10, 10);
                getParameters["o_telefono_celular"].Value = "+5491177788877";
                getParameters["o_email"].Value = "ramon@example.com";
                getParameters["o_fecha_creacion"].Value = new System.DateTime(2024, 4, 1);
                getParameters["o_fecha_modificacion"].Value = new System.DateTime(2024, 4, 2);
            })
            .ReturnsAsync(1);

        var commandQueue = new Queue<INpgsqlCommandWrapper>(new[] { updateCommandMock.Object, getCommandMock.Object });
        _connectionMock
            .Setup(c => c.CreateCommand(It.IsAny<string>(), It.IsAny<INpgsqlTransactionWrapper>()))
            .Returns(() => commandQueue.Dequeue());
        _connectionMock
            .Setup(c => c.CreateCommand(It.Is<string>(s => s == "sp_clientes_get_by_id"), null))
            .Returns(getCommandMock.Object);

        _connectionFactoryMock
            .Setup(f => f.CreateConnection(It.IsAny<string>()))
            .Returns(_connectionMock.Object);

        var repository = new ClienteWriteRepository(_configuration, NullLogger<ClienteWriteRepository>.Instance, _connectionFactoryMock.Object);

        var cliente = await repository.UpdateAsync(8, "Ramon", "Diaz", "RD SA", "20-33333333-3", new DateOnly(1980, 10, 10), "+5491177788877", "ramon@example.com");

        cliente.Id.Should().Be(8);
        updateParameters.Should().Contain(p => p.ParameterName == "p_id" && (int)p.Value == 8);
    }

    [Test]
    public void InsertAsync_CuandoExisteConflicto_ArrojaClienteConflictException()
    {
        var insertCommandMock = CreateCommandMock();
        var postgresException = new PostgresException(
            messageText: "Ya existe un cliente con el CUIT",
            severity: "ERROR",
            invariantSeverity: "ERROR",
            sqlState: "23505",
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

        insertCommandMock
            .Setup(c => c.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(postgresException);

        _connectionMock
            .Setup(c => c.CreateCommand(It.IsAny<string>(), It.IsAny<INpgsqlTransactionWrapper>()))
            .Returns(insertCommandMock.Object);
        _connectionFactoryMock
            .Setup(f => f.CreateConnection(It.IsAny<string>()))
            .Returns(_connectionMock.Object);

        var repository = new ClienteWriteRepository(_configuration, _loggerMock.Object, _connectionFactoryMock.Object);

        var action = async () => await repository.InsertAsync("Luis", "Martinez", "LM SRL", "20-22222222-2", new DateOnly(1985, 12, 1), "+5491188899988", "luis@example.com");

        action.Should().ThrowAsync<ClienteConflictException>();
        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, _) => o.ToString()!.Contains("Error de base de datos al insertar cliente")),
                postgresException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public void InsertAsync_CuandoProcedimientoNoDevuelveId_LanzaInvalidOperationException()
    {
        var insertCommandMock = CreateCommandMock();
        var insertParameters = insertCommandMock.Object.Parameters;

        insertCommandMock
            .Setup(c => c.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()))
            .Callback(() => insertParameters["p_id"].Value = DBNull.Value)
            .ReturnsAsync(1);

        _connectionMock
            .Setup(c => c.CreateCommand(It.IsAny<string>(), It.IsAny<INpgsqlTransactionWrapper>()))
            .Returns(insertCommandMock.Object);
        _connectionFactoryMock
            .Setup(f => f.CreateConnection(It.IsAny<string>()))
            .Returns(_connectionMock.Object);

        var repository = new ClienteWriteRepository(_configuration, NullLogger<ClienteWriteRepository>.Instance, _connectionFactoryMock.Object);

        var action = async () => await repository.InsertAsync("n", "a", "r", "c", new DateOnly(2000, 1, 1), "t", "e");

        action.Should().ThrowAsync<InvalidOperationException>();
    }

    [Test]
    public void DeleteAsync_CuandoNoAfectaFilas_ArrojaClienteNotFoundException()
    {
        var deleteCommandMock = CreateCommandMock();
        deleteCommandMock
            .Setup(c => c.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()))
            .Callback(() => deleteCommandMock.Object.Parameters["p_rows_affected"].Value = 0)
            .ReturnsAsync(0);

        _connectionMock
            .Setup(c => c.CreateCommand(It.IsAny<string>(), It.IsAny<INpgsqlTransactionWrapper>()))
            .Returns(deleteCommandMock.Object);
        _connectionFactoryMock
            .Setup(f => f.CreateConnection(It.IsAny<string>()))
            .Returns(_connectionMock.Object);

        var repository = new ClienteWriteRepository(_configuration, NullLogger<ClienteWriteRepository>.Instance, _connectionFactoryMock.Object);

        var action = async () => await repository.DeleteAsync(10);

        action.Should().ThrowAsync<ClienteNotFoundException>();
    }

    [Test]
    public void UpdateAsync_CuandoPostgresLanzaOtroError_LanzaPostgresException()
    {
        var updateCommandMock = CreateCommandMock();
        var postgresException = new PostgresException(
            messageText: "Error desconocido",
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

        updateCommandMock
            .Setup(c => c.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(postgresException);

        _connectionMock
            .Setup(c => c.CreateCommand(It.IsAny<string>(), It.IsAny<INpgsqlTransactionWrapper>()))
            .Returns(updateCommandMock.Object);
        _connectionFactoryMock
            .Setup(f => f.CreateConnection(It.IsAny<string>()))
            .Returns(_connectionMock.Object);

        var repository = new ClienteWriteRepository(_configuration, NullLogger<ClienteWriteRepository>.Instance, _connectionFactoryMock.Object);

        var action = async () => await repository.UpdateAsync(1, "n", "a", "r", "c", new DateOnly(2000, 1, 1), "t", "e");

        action.Should().ThrowAsync<PostgresException>();
    }

    [Test]
    public void UpdateAsync_CuandoPostgresIndicaConflictoEmail_LanzaClienteConflictException()
    {
        var updateCommandMock = CreateCommandMock();
        var postgresException = new PostgresException(
            messageText: "Ya existe un cliente con el email",
            severity: "ERROR",
            invariantSeverity: "ERROR",
            sqlState: "23505",
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

        updateCommandMock
            .Setup(c => c.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(postgresException);

        _connectionMock
            .Setup(c => c.CreateCommand(It.IsAny<string>(), It.IsAny<INpgsqlTransactionWrapper>()))
            .Returns(updateCommandMock.Object);
        _connectionFactoryMock
            .Setup(f => f.CreateConnection(It.IsAny<string>()))
            .Returns(_connectionMock.Object);

        var repository = new ClienteWriteRepository(_configuration, _loggerMock.Object, _connectionFactoryMock.Object);

        var action = async () => await repository.UpdateAsync(2, "n", "a", "r", "c", new DateOnly(2000, 1, 1), "t", "e");

        action.Should().ThrowAsync<ClienteConflictException>();
        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, _) => o.ToString()!.Contains("Error de base de datos al actualizar cliente")),
                postgresException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public void UpdateAsync_CuandoPostgresIndicaNoEncontrado_LanzaClienteNotFoundException()
    {
        var updateCommandMock = CreateCommandMock();
        var postgresException = new PostgresException(
            messageText: "No existe un cliente con el id 15",
            severity: "ERROR",
            invariantSeverity: "ERROR",
            sqlState: "P0002",
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

        updateCommandMock
            .Setup(c => c.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(postgresException);

        _connectionMock
            .Setup(c => c.CreateCommand(It.IsAny<string>(), It.IsAny<INpgsqlTransactionWrapper>()))
            .Returns(updateCommandMock.Object);
        _connectionFactoryMock
            .Setup(f => f.CreateConnection(It.IsAny<string>()))
            .Returns(_connectionMock.Object);

        var repository = new ClienteWriteRepository(_configuration, _loggerMock.Object, _connectionFactoryMock.Object);

        var action = async () => await repository.UpdateAsync(15, "n", "a", "r", "c", new DateOnly(2000, 1, 1), "t", "e");

        action.Should().ThrowAsync<ClienteNotFoundException>();
        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, _) => o.ToString()!.Contains("Error de base de datos al actualizar cliente")),
                postgresException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public void InsertAsync_CuandoOcurreExcepcionGenerica_LoggeaYPropaga()
    {
        var insertCommandMock = CreateCommandMock();

        insertCommandMock
            .Setup(c => c.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("unexpected"));

        _connectionMock
            .Setup(c => c.CreateCommand(It.IsAny<string>(), It.IsAny<INpgsqlTransactionWrapper>()))
            .Returns(insertCommandMock.Object);
        _connectionFactoryMock
            .Setup(f => f.CreateConnection(It.IsAny<string>()))
            .Returns(_connectionMock.Object);

        var repository = new ClienteWriteRepository(_configuration, _loggerMock.Object, _connectionFactoryMock.Object);

        var action = async () => await repository.InsertAsync("a", "b", "c", "d", new DateOnly(2000, 1, 1), "t", "e");

        action.Should().ThrowAsync<Exception>();
        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, _) => o.ToString()!.Contains("Error inesperado al insertar cliente")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public void UpdateAsync_CuandoNoAfectaFilas_LanzaClienteNotFoundException()
    {
        var updateCommandMock = CreateCommandMock();

        updateCommandMock
            .Setup(c => c.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()))
            .Callback(() => updateCommandMock.Object.Parameters["p_rows_affected"].Value = 0)
            .ReturnsAsync(0);

        _connectionMock
            .Setup(c => c.CreateCommand(It.IsAny<string>(), It.IsAny<INpgsqlTransactionWrapper>()))
            .Returns(updateCommandMock.Object);
        _connectionFactoryMock
            .Setup(f => f.CreateConnection(It.IsAny<string>()))
            .Returns(_connectionMock.Object);

        var repository = new ClienteWriteRepository(_configuration, _loggerMock.Object, _connectionFactoryMock.Object);

        var action = async () => await repository.UpdateAsync(3, "n", "a", "r", "c", new DateOnly(2000, 1, 1), "t", "e");

        action.Should().ThrowAsync<ClienteNotFoundException>();
    }

    [Test]
    public void UpdateAsync_CuandoExcepcionGenerica_LoggeaYPropaga()
    {
        var updateCommandMock = CreateCommandMock();

        updateCommandMock
            .Setup(c => c.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("boom"));

        _connectionMock
            .Setup(c => c.CreateCommand(It.IsAny<string>(), It.IsAny<INpgsqlTransactionWrapper>()))
            .Returns(updateCommandMock.Object);
        _connectionFactoryMock
            .Setup(f => f.CreateConnection(It.IsAny<string>()))
            .Returns(_connectionMock.Object);

        var repository = new ClienteWriteRepository(_configuration, _loggerMock.Object, _connectionFactoryMock.Object);

        var action = async () => await repository.UpdateAsync(4, "n", "a", "r", "c", new DateOnly(2000, 1, 1), "t", "e");

        action.Should().ThrowAsync<Exception>();
        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, _) => o.ToString()!.Contains("Error inesperado al actualizar cliente")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public async Task DeleteAsync_CuandoEliminaCliente_NoLanzaExcepcion()
    {
        var deleteCommandMock = CreateCommandMock();

        deleteCommandMock
            .Setup(c => c.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()))
            .Callback(() => deleteCommandMock.Object.Parameters["p_rows_affected"].Value = 1)
            .ReturnsAsync(1);

        _connectionMock
            .Setup(c => c.CreateCommand(It.IsAny<string>(), It.IsAny<INpgsqlTransactionWrapper>()))
            .Returns(deleteCommandMock.Object);
        _connectionFactoryMock
            .Setup(f => f.CreateConnection(It.IsAny<string>()))
            .Returns(_connectionMock.Object);

        var repository = new ClienteWriteRepository(_configuration, _loggerMock.Object, _connectionFactoryMock.Object);

        await repository.DeleteAsync(5);

        deleteCommandMock.Object.Parameters.Should().Contain(p => p.ParameterName == "p_id" && (int)p.Value == 5);
    }

    [Test]
    public void DeleteAsync_CuandoPostgresFalla_LoggeaYPropaga()
    {
        var deleteCommandMock = CreateCommandMock();
        var postgresException = new PostgresException(
            messageText: "db error",
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

        deleteCommandMock
            .Setup(c => c.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(postgresException);

        _connectionMock
            .Setup(c => c.CreateCommand(It.IsAny<string>(), It.IsAny<INpgsqlTransactionWrapper>()))
            .Returns(deleteCommandMock.Object);
        _connectionFactoryMock
            .Setup(f => f.CreateConnection(It.IsAny<string>()))
            .Returns(_connectionMock.Object);

        var repository = new ClienteWriteRepository(_configuration, _loggerMock.Object, _connectionFactoryMock.Object);

        var action = async () => await repository.DeleteAsync(6);

        action.Should().ThrowAsync<PostgresException>();
        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, _) => o.ToString()!.Contains("Error de base de datos al eliminar cliente")),
                postgresException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public void DeleteAsync_CuandoPostgresIndicaNoEncontrado_LanzaClienteNotFoundException()
    {
        var deleteCommandMock = CreateCommandMock();
        var postgresException = new PostgresException(
            messageText: "No existe un cliente con el id 9",
            severity: "ERROR",
            invariantSeverity: "ERROR",
            sqlState: "P0002",
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

        deleteCommandMock
            .Setup(c => c.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(postgresException);

        _connectionMock
            .Setup(c => c.CreateCommand(It.IsAny<string>(), It.IsAny<INpgsqlTransactionWrapper>()))
            .Returns(deleteCommandMock.Object);
        _connectionFactoryMock
            .Setup(f => f.CreateConnection(It.IsAny<string>()))
            .Returns(_connectionMock.Object);

        var repository = new ClienteWriteRepository(_configuration, _loggerMock.Object, _connectionFactoryMock.Object);

        var action = async () => await repository.DeleteAsync(9);

        action.Should().ThrowAsync<ClienteNotFoundException>();
        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, _) => o.ToString()!.Contains("Error de base de datos al eliminar cliente")),
                postgresException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public void DeleteAsync_CuandoExcepcionGenerica_LoggeaYPropaga()
    {
        var deleteCommandMock = CreateCommandMock();

        deleteCommandMock
            .Setup(c => c.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("boom"));

        _connectionMock
            .Setup(c => c.CreateCommand(It.IsAny<string>(), It.IsAny<INpgsqlTransactionWrapper>()))
            .Returns(deleteCommandMock.Object);
        _connectionFactoryMock
            .Setup(f => f.CreateConnection(It.IsAny<string>()))
            .Returns(_connectionMock.Object);

        var repository = new ClienteWriteRepository(_configuration, _loggerMock.Object, _connectionFactoryMock.Object);

        var action = async () => await repository.DeleteAsync(7);

        action.Should().ThrowAsync<Exception>();
        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, _) => o.ToString()!.Contains("Error inesperado al eliminar cliente")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public void GetByIdAsync_CuandoProcedimientoNoRetornaDatos_LanzaInvalidOperationException()
    {
        var commandMock = CreateCommandMockForGetById();
        commandMock
            .Setup(c => c.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _connectionMock
            .Setup(c => c.CreateCommand(It.IsAny<string>(), null))
            .Returns(commandMock.Object);

        var method = typeof(ClienteWriteRepository).GetMethod(
            "GetByIdAsync",
            BindingFlags.NonPublic | BindingFlags.Static);

        method.Should().NotBeNull();

        var action = async () => await (Task<Cliente>)method!.Invoke(null, new object[] { _connectionMock.Object, 30, CancellationToken.None })!;

        action.Should().ThrowAsync<InvalidOperationException>();
    }

    private static Mock<INpgsqlCommandWrapper> CreateCommandMock()
    {
        var commandMock = new Mock<INpgsqlCommandWrapper>();
        var parameters = new NpgsqlCommand().Parameters;
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

    private static Mock<INpgsqlCommandWrapper> CreateCommandMockForGetById()
    {
        var commandMock = new Mock<INpgsqlCommandWrapper>();
        var parameters = new NpgsqlCommand().Parameters;
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
}
