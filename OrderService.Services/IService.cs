using OrderService.Common;

namespace OrderService.Services;

public interface IService<T, TError>
{
  Task<Result<T, TError>> ExecuteAsync(CancellationToken cancellationToken);


}