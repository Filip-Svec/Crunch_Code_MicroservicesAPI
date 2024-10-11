namespace MicroservicesAPI.Shared.Exceptions;

public class TypeMismatchException : Exception
{
    public TypeMismatchException(string message) : base(message) { }
}
