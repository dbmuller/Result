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

    public static Task<Result> FromAsync<T>(Result<T> result)
        => Task.FromResult<Result>(new(result.Success, result.Errors));

    public static readonly Result Successful = new();

    // Coming from the generic version to return just fail
    public static Result CreateError<T>(Result<T> error) => new(error.Errors);
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
    public Result<TReturn> HandleReturnResult<TReturn>(Func<T, Result<TReturn>> onSuccess, Func<Result<T>, Result<TReturn>> onError)
        => Success ? onSuccess(_data) : onError(this);

    public Result HandleReturnResult(Func<T, Result> onSuccess, Func<Result<T>, Result> onError)
        => Success ? onSuccess(_data) : onError(this);

    /// <summary>
    /// Handle the result async and pass the result back down to handle further
    /// </summary>
    public Task<Result<TReturn>> HandleReturnResultAsync<TReturn>(Func<T, Task<Result<TReturn>>> onSuccess, Func<Result<T>, Task<Result<TReturn>>> onError)
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

