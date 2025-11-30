using Clientes.Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace Clientes.Tests.Clientes.Domain;

[TestFixture]
public class ClienteTests
{
    [Test]
    public void Constructor_ShouldAssignAllProperties()
    {
        // Arrange & Act
        var cliente = new Cliente
        {
            Id = 5,
            Nombre = "Ana",
            Apellido = "García",
            RazonSocial = "Ana SRL",
            Cuit = "20-12345678-9",
            FechaNacimiento = new DateOnly(1995, 12, 31),
            TelefonoCelular = "+5491100000000",
            Email = "ana@example.com",
            FechaCreacion = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc),
            FechaModificacion = new DateTime(2024, 2, 1, 11, 30, 0, DateTimeKind.Utc)
        };

        // Assert
        cliente.Id.Should().Be(5);
        cliente.Nombre.Should().Be("Ana");
        cliente.Apellido.Should().Be("García");
        cliente.RazonSocial.Should().Be("Ana SRL");
        cliente.Cuit.Should().Be("20-12345678-9");
        cliente.FechaNacimiento.Should().Be(new DateOnly(1995, 12, 31));
        cliente.TelefonoCelular.Should().Be("+5491100000000");
        cliente.Email.Should().Be("ana@example.com");
        cliente.FechaCreacion.Should().Be(new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc));
        cliente.FechaModificacion.Should().Be(new DateTime(2024, 2, 1, 11, 30, 0, DateTimeKind.Utc));
    }

    [Test]
    public void DefaultConstructor_ShouldInitializeStringsAsEmpty()
    {
        // Act
        var cliente = new Cliente();

        // Assert
        cliente.Nombre.Should().BeEmpty();
        cliente.Apellido.Should().BeEmpty();
        cliente.RazonSocial.Should().BeEmpty();
        cliente.Cuit.Should().BeEmpty();
        cliente.TelefonoCelular.Should().BeEmpty();
        cliente.Email.Should().BeEmpty();
    }
}
