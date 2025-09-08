using System.Diagnostics.CodeAnalysis;

namespace MildlySublime.Result.Core;

public sealed class Result
{
    private Result()
    {
        Success = true;
    }

    private Result(IEnumerable<ResultError> error)
    {
        Success = false;
        Errors.AddRange(error);
    }

    internal bool Success { get; }

    public readonly List<ResultError> Errors = [];

    public TReturn HandleResult<TReturn>(Func<TReturn> onSuccess, Func<Result, TReturn> onError)
        => Success ? onSuccess() : onError(this);

    public static Result From<T>(Result<T> result)
        => result.HandleReturnResult(onSuccess: _ => Result.Successful, onError: CreateError);

    public static Result Successful => new();

    public static Result CreateError<T>(Result<T> result) => new(result.Errors);
    public static Result CreateError(IEnumerable<ResultError> error) => new(error);
    public static Result CreateError(IEnumerable<Result> result) => new(result.Errors);
    public static Result CreateError<T>(IEnumerable<Result<T>> result) => new(result.Errors);
}

public sealed class Result<T>
{
    private Result(T value)
    {
        Success = true;
        _data = value;
    }

    private Result(IEnumerable<ResultError> error)
    {
        Success = false;
        _data = default;
        Errors.AddRange(error);
    }

    [MemberNotNullWhen(true, nameof(_data))]
    internal bool Success { get; }

    private readonly T? _data;
    public readonly List<ResultError> Errors = [];

    /// <summary>
    /// To pass the result back down to handle further
    /// </summary>
    public TReturn HandleReturnResult<TReturn>(Func<T, TReturn> onSuccess, Func<Result<T>, TReturn> onError)
        => Success ? onSuccess(_data) : onError(this);

    public Task<TReturn> HandleReturnResultAsync<TReturn>(Func<T, Task<TReturn>> onSuccess, Func<Result<T>, Task<TReturn>> onError)
        => Success ? onSuccess(_data) : onError(this);

    public void HandleResult(Action<T> onSuccess, Action<Result<T>> onError)
    {
        if (Success)
            onSuccess(_data);
        else
            onError(this);
    }

    public Task HandleResultAsync(Func<T, Task> onSuccess, Func<Result<T>, Task> onError)
        => Success ? onSuccess(_data) : onError(this);

    public static Result<T> CreateSuccess(T value) => new(value);
    public static Result<T> CreateError(IEnumerable<ResultError> error) => new(error);
    public static Result<T> CreateError(Result<T> result) => new(result.Errors);

    public static implicit operator Result<T>(T value) => CreateSuccess(value);
}

