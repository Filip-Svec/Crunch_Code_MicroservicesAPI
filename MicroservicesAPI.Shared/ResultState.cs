namespace MicroservicesAPI.Shared;

public enum ResultState
{
    SyntaxError,
    TimeLimitExceeded,
    OutOfMemory,
    TypeMismatch,
    ValueMismatch,
    Success,
    Unknown
}