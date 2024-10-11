namespace MicroservicesAPI.Shared;

public enum ResultState
{
    // Execution fine, result produced
    Success,
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