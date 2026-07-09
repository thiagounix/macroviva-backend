namespace MacroViva.Application.Common;

public class Result
{
    protected Result(bool isSuccess, Error? error)
    {
        if (isSuccess && error is not null)
        {
            throw new InvalidOperationException("Successful results cannot have an error.");
        }

        if (!isSuccess && error is null)
        {
            throw new InvalidOperationException("Failed results must have an error.");
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error? Error { get; }

    public static Result Success()
    {
        return new Result(true, null);
    }

    public static Result Failure(Error error)
    {
        return new Result(false, error);
    }
}
