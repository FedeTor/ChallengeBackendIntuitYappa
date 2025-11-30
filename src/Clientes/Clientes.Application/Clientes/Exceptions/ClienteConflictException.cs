namespace Clientes.Application.Clientes.Exceptions;

public class ClienteConflictException : Exception
{
    public ClienteConflictException(string message) : base(message)
    {
    }
}
