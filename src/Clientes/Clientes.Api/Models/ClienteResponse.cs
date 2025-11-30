namespace Clientes.Api.Models;

/// <summary>
/// Datos de un cliente devuelto por la API.
/// </summary>
public class ClienteResponse
{
    /// <summary>
    /// Identificador del cliente.
    /// </summary>
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Apellido { get; set; } = string.Empty;

    public string RazonSocial { get; set; } = string.Empty;

    public string Cuit { get; set; } = string.Empty;

    public DateOnly FechaNacimiento { get; set; }

    public string TelefonoCelular { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaModificacion { get; set; }
}
