using MildlySublime.Result.Core;

namespace MildlySublime.Result.Core;

public static class ResultExtensions
{
    extension(IEnumerable<Result> results)
    {
        public IEnumerable<ResultError> Errors
            => results.Where(p => !p.Success).SelectMany(p => p.Errors!);

        public static Result CreateSuccess() => Result.Successful;
    }

    extension<T>(IEnumerable < Result < T >> results)
    {
        public IEnumerable<ResultError> Errors
            => results.Where(p => !p.Success).SelectMany(p => p.Errors!);

        public static Result CreateSuccess() => Result.Successful;
    }
}
