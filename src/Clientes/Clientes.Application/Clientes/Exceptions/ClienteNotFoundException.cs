namespace Clientes.Application.Clientes.Exceptions;

public class ClienteNotFoundException : Exception
{
    public ClienteNotFoundException(string message) : base(message)
    {
    }
}
