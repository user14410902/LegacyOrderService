using OrderService.Common;
using OrderService.Entities;

namespace OrderService.Services.Services;

public interface IOrderRetrievalService
{
  Task<Result<List<Order>, string>> ExecuteAsync(CancellationToken cancellationToken);


}