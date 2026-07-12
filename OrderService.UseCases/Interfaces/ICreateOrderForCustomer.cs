using OrderService.Common;
using OrderService.Entities;

using ResultType = OrderService.Common.Result<OrderService.Entities.Order, string>;

namespace OrderService.UseCases.Implementations;

public interface ICreateOrderForCustomer
{

  public ResultType Execute(string customerName, Product product, int quantity);
}