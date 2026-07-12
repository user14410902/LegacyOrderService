using OrderService.Common;
using OrderService.Entities;
using OrderService.Services.AddOrder;

namespace OrderService.Services.Interfaces;

public interface IOrderRetrievalService
{
  Task<Result<List<Order>, string>> ExecuteAsync(CancellationToken cancellationToken);


}