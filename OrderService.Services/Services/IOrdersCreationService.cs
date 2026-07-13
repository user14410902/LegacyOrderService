using OrderService.Common;
using OrderService.Services.Sources;

namespace OrderService.Services.Services;

public interface IOrdersCreationService
{
  Task<Result<bool, List<string>>> ExecuteAsync(IRowsSource source, CancellationToken cancellationToken);


}