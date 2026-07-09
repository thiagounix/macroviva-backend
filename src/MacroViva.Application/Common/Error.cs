namespace MacroViva.Application.Common;

public sealed record Error(string Code, string Message, ErrorType Type)
{
    public static Error Validation(string code, string message)
    {
        return new Error(code, message, ErrorType.Validation);
    }

    public static Error NotFound(string code, string message)
    {
        return new Error(code, message, ErrorType.NotFound);
    }

    public static Error Unauthorized(string code, string message)
    {
        return new Error(code, message, ErrorType.Unauthorized);
    }

    public static Error Conflict(string code, string message)
    {
        return new Error(code, message, ErrorType.Conflict);
    }

    public static Error Failure(string code, string message)
    {
        return new Error(code, message, ErrorType.Failure);
    }
}
