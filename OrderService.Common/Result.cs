namespace OrderService.Common;

public record Result<T, TError>
{
  public bool IsSuccess { get; }
  public T Value { get; }
  public TError? Error { get; }

  private Result(bool isSuccess, T value, TError? error)
  {
    IsSuccess = isSuccess;
    Value = value!;
    Error = error;
  }

  public static Result<T, TError> Success(T value) => new Result<T, TError>(true, value, default);
  public static Result<T, TError> Failure(TError error) => new Result<T, TError>(false, default!, error);
}
