namespace MicroservicesAPI.Shared.Exceptions;

public class ValueMismatchException : Exception
{
    public ValueMismatchException(string message) : base(message) { }
}