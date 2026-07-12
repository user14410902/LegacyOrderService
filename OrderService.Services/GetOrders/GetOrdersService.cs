using Microsoft.Extensions.Logging;
using OrderService.Data.Interfaces;
using OrderService.Entities;
using OrderService.Services.Interfaces;
using ResultType = OrderService.Common.Result<System.Collections.Generic.List<OrderService.Entities.Order>, string>;

namespace OrderService.Services.GetOrders;

public class GetOrdersService(IOrderRepository orderRepository) : IOrderRetrievalService
{
  private readonly IOrderRepository _orderRepository = orderRepository;

  public async Task<Common.Result<List<Order>, string>> ExecuteAsync(CancellationToken cancellationToken)
  {
    var orders = await _orderRepository.GetAllOrdersAsync(cancellationToken);
    return ResultType.Success(orders);
  }
}