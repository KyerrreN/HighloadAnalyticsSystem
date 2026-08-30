namespace Telemetry.Contracts.Result;

public record Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException("Success result cannot contain an error.");

        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException("Failure result must contain an error.");

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, Error.None);
    public static Result Failed(Error error) => new(false, error);

    public static Result<T> Success<T>(T value) => Result<T>.Success(value);
    public static Result<T> Failed<T>(Error error) => Result<T>.Failed(error);
}

public record Result<T> : Result
{
    public T? Value { get; }

    private Result(T? value, bool isSuccess, Error error) : base(isSuccess, error)
    {
        Value = value;
    }

    public static new Result<T> Success(T value) => new(value, true, Error.None);

    public static new Result<T> Failed(Error error) => new(default, false, error);

    public static implicit operator Result<T>(T value) => Success(value);
}
