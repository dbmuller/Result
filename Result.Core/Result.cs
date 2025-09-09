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

    /// <summary>
    /// when copying a result, such as from Result<T>
    /// </summary>
    private Result(bool success, IEnumerable<ResultError> errors)
    {
        Success = success;
        Errors.AddRange(errors);
    }

    internal bool Success { get; }

    public readonly List<ResultError> Errors = [];

    public Result HandleResult(Func<Result> onSuccess, Func<Result, Result> onError)
        => Success ? onSuccess() : onError(this);
    
    public Task<Result> HandleResultAsync(Func<Task<Result>> onSuccess, Func<Result, Task<Result>> onError)
        => Success ? onSuccess() : onError(this);

    public static Result From<T>(Result<T> result)
        => new(result.Success, result.Errors);

    public static readonly Result Successful = new();

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
    /// Handle the result and pass the result back down to handle further
    /// </summary>
    public Result<T> HandleReturnResult(Func<T, Result<T>> onSuccess, Func<Result<T>, Result<T>> onError)
        => Success ? onSuccess(_data) : onError(this);

    /// <summary>
    /// Handle the result async and pass the result back down to handle further
    /// </summary>
    public Task<Result<T>> HandleReturnResultAsync(Func<T, Task<Result<T>>> onSuccess, Func<Result<T>, Task<Result<T>>> onError)
        => Success ? onSuccess(_data) : onError(this);

    /// <summary>
    /// No return, just handle the result
    /// </summary>
    public void HandleResult(Action<T> onSuccess, Action<Result<T>> onError)
    {
        if (Success)
            onSuccess(_data);
        else
            onError(this);
    }

    /// <summary>
    /// No return, just handle the result async
    /// </summary>
    public Task HandleResultAsync(Func<T, Task> onSuccess, Func<Result<T>, Task> onError)
        => Success ? onSuccess(_data) : onError(this);

    public static Result<T> CreateSuccess(T value) => new(value);
    public static Result<T> CreateError(IEnumerable<ResultError> error) => new(error);
    public static Result<T> CreateError(Result<T> result) => new(result.Errors);

    public static implicit operator Result<T>(T value) => CreateSuccess(value);
}

