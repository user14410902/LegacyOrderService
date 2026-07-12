using OrderService.Common;
using OrderService.Services.AddOrder;

namespace OrderService.Services.Interfaces;

public interface IOrderCreationService
{
  Task<Result<Guid, string>> ExecuteAsync(IAddOrderSource source, CancellationToken cancellationToken);


}