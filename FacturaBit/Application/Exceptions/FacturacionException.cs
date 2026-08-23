namespace FacturaBit.Application.Exceptions;

public sealed class FacturacionException : Exception
{
    public FacturacionException(string message) : base(message)
    {
    }

    public FacturacionException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
