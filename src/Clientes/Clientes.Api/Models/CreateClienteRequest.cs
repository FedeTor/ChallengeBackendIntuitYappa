using System.ComponentModel.DataAnnotations;

namespace Clientes.Api.Models;

/// <summary>
/// Datos requeridos para registrar un nuevo cliente.
/// </summary>
public class CreateClienteRequest
{
    /// <summary>
    /// Nombre del cliente.
    /// </summary>
    [Required]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Apellido del cliente.
    /// </summary>
    [Required]
    public string Apellido { get; set; } = string.Empty;

    /// <summary>
    /// Razón social asociada al cliente.
    /// </summary>
    [Required]
    public string RazonSocial { get; set; } = string.Empty;

    /// <summary>
    /// CUIT del cliente en formato 99-99999999-9.
    /// </summary>
    [Required]
    [RegularExpression("^[0-9]{2}-[0-9]{8}-[0-9]$", ErrorMessage = "El formato de CUIT debe ser 99-99999999-9")]
    public string Cuit { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de nacimiento.
    /// </summary>
    [Required]
    public DateOnly FechaNacimiento { get; set; }

    /// <summary>
    /// Teléfono celular de contacto.
    /// </summary>
    [Required]
    public string TelefonoCelular { get; set; } = string.Empty;

    /// <summary>
    /// Correo electrónico del cliente.
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
