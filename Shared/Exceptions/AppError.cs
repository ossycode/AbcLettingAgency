namespace AbcLettingAgency.Shared.Exceptions;

public enum ErrorType
{
    Validation = 2,   // 422
    NotFound = 1,     // 404
    Unauthorized = 4, // 401
    Forbidden = 5,    // 403
    Conflict = 3,     // 409
    Failure = 0      // 400 generic/other domain failures
}

public sealed class AppError
{
    public string Code { get; }        
    public string Message { get; }    
    public string? Key { get; }         
    public ErrorType Type { get; }
    public IReadOnlyList<KeyValuePair<string, string>>? Details { get; init; }

    public AppError(string code, string message, ErrorType type = ErrorType.Failure, string? key = null)
    {
        Code = code;
        Message = message;
        Type = type;
        Key = string.IsNullOrWhiteSpace(key) ? null : key;
    }
}

public class Result
{
    public bool IsSuccess { get; }
    public IReadOnlyList<AppError> Errors { get; }

    protected Result(bool ok, IEnumerable<AppError>? errors = null)
    {
        IsSuccess = ok;
        Errors = ok ? Array.Empty<AppError>() : (errors?.ToList() ?? new List<AppError>(1));
    }

    public static Result Success() => new(true);
    public static Result Failure(params AppError[] errors) => new(false, errors);
    public static Result Failure(IEnumerable<AppError> errors) => new(false, errors);
}

public sealed class Result<T> : Result
{
    public T? Value { get; }

    private Result(T value) : base(true) => Value = value;
    private Result(IEnumerable<AppError> errors) : base(false, errors) => Value = default;

    public static Result<T> Success(T value) => new(value);
    public static new Result<T> Failure(params AppError[] errors) => new(errors);
    public static new Result<T> Failure(IEnumerable<AppError> errors) => new(errors);

    public static implicit operator Result<T>(T value) => Success(value);
    public static implicit operator Result<T>(AppError error) => Failure(error);
}

