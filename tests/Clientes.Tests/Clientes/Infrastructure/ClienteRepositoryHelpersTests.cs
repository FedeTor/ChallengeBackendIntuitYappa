using System;
using System.Reflection;
using Clientes.Domain.Entities;
using Clientes.Infrastructure.Repositories;
using FluentAssertions;
using Npgsql;
using NUnit.Framework;

namespace Clientes.Tests.Clientes.Infrastructure;

[TestFixture]
public class ClienteRepositoryHelpersTests
{
    [Test]
    public void ToDateOnly_ShouldConvertDateTime()
    {
        // Arrange
        var dateTime = new DateTime(2024, 3, 15, 8, 30, 0);

        // Act
        var result = InvokeToDateOnly(typeof(ClienteReadRepository), dateTime);

        // Assert
        result.Should().Be(new DateOnly(2024, 3, 15));
    }

    [Test]
    public void ToDateOnly_ShouldConvertString()
    {
        // Arrange
        var value = "2024-04-20";

        // Act
        var result = InvokeToDateOnly(typeof(ClienteWriteRepository), value);

        // Assert
        result.Should().Be(new DateOnly(2024, 4, 20));
    }

    [Test]
    public void ToDateOnly_WithDateOnlyValue_ReturnsSameValue()
    {
        // Arrange
        var dateOnly = new DateOnly(2023, 12, 24);

        // Act
        var result = InvokeToDateOnly(typeof(ClienteWriteRepository), dateOnly);

        // Assert
        result.Should().Be(dateOnly);
    }

    [Test]
    public void ToDateOnly_ShouldThrow_WhenNull()
    {
        // Act
        var result = InvokeToDateOnly(typeof(ClienteReadRepository), null!);

        // Assert
        result.Should().Be(DateOnly.FromDateTime(DateTime.MinValue));
    }

    [Test]
    public void MapCliente_ShouldMapFromParameterCollection_ReadRepository()
    {
        // Arrange
        var parameters = BuildParameters();

        // Act
        var result = InvokeMapCliente(typeof(ClienteReadRepository), parameters);

        // Assert
        AssertClienteMapping(result);
    }

    [Test]
    public void MapCliente_ShouldMapFromParameterCollection_WriteRepository()
    {
        // Arrange
        var parameters = BuildParameters();

        // Act
        var result = InvokeMapCliente(typeof(ClienteWriteRepository), parameters);

        // Assert
        AssertClienteMapping(result);
    }

    private static Cliente InvokeMapCliente(Type type, NpgsqlParameterCollection parameters)
    {
        var method = type.GetMethod(
            "MapCliente",
            BindingFlags.NonPublic | BindingFlags.Static,
            binder: null,
            types: new[] { typeof(NpgsqlParameterCollection) },
            modifiers: null);
        method.Should().NotBeNull();

        var cliente = method!.Invoke(null, new object[] { parameters });
        cliente.Should().BeOfType<Cliente>();

        return (Cliente)cliente!;
    }

    private static DateOnly InvokeToDateOnly(Type type, object value)
    {
        var method = type.GetMethod("ToDateOnly", BindingFlags.NonPublic | BindingFlags.Static);
        method.Should().NotBeNull();

        var result = method!.Invoke(null, new[] { value });
        result.Should().BeOfType<DateOnly>();

        return (DateOnly)result!;
    }

    private static NpgsqlParameterCollection BuildParameters()
    {
        var command = new NpgsqlCommand();
        command.Parameters.AddWithValue("o_id", 10);
        command.Parameters.AddWithValue("o_nombre", "Laura");
        command.Parameters.AddWithValue("o_apellido", "Suarez");
        command.Parameters.AddWithValue("o_razon_social", "LS SA");
        command.Parameters.AddWithValue("o_cuit", "20-11111111-1");
        command.Parameters.AddWithValue("o_fecha_nacimiento", new DateTime(1991, 5, 5));
        command.Parameters.AddWithValue("o_telefono_celular", "+541199999999");
        command.Parameters.AddWithValue("o_email", "laura@example.com");
        command.Parameters.AddWithValue("o_fecha_creacion", new DateTime(2024, 1, 10));
        command.Parameters.AddWithValue("o_fecha_modificacion", new DateTime(2024, 2, 10));
        return command.Parameters;
    }

    private static void AssertClienteMapping(Cliente cliente)
    {
        cliente.Id.Should().Be(10);
        cliente.Nombre.Should().Be("Laura");
        cliente.Apellido.Should().Be("Suarez");
        cliente.RazonSocial.Should().Be("LS SA");
        cliente.Cuit.Should().Be("20-11111111-1");
        cliente.FechaNacimiento.Should().Be(new DateOnly(1991, 5, 5));
        cliente.TelefonoCelular.Should().Be("+541199999999");
        cliente.Email.Should().Be("laura@example.com");
        cliente.FechaCreacion.Should().Be(new DateTime(2024, 1, 10));
        cliente.FechaModificacion.Should().Be(new DateTime(2024, 2, 10));
    }
}
