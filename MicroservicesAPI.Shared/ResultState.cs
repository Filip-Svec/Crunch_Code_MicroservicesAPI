namespace MicroservicesAPI.Shared;

public enum ResultState
{
    // Execution fine, result produced
    Success,
    OutputLengthExceeded,
    TypeMismatch,
    ValueMismatch,
    
    // Execution fail, no result
    SyntaxError,
    ArgumentType,
    DivideByZero,
    UnboundName,
    TimeLimitExceeded,
    OutOfMemory,
    Other
}