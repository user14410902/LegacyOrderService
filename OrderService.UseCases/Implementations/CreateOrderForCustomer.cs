using OrderService.Common;
using OrderService.Entities;

using ResultType = OrderService.Common.Result<OrderService.Entities.Order, string>;

namespace OrderService.UseCases.Implementations;

public class CreateOrderForCustomer : ICreateOrderForCustomer
{

  public ResultType Execute(string customerName, Product product, int quantity)
  {
    if (string.IsNullOrWhiteSpace(customerName))
    {
      return ResultType.Failure("CustomerName is null or empty. CustomerName is required.");
    }

    if (product == null)
    {
      return ResultType.Failure("Product is null. Product is required.");
    }

    if (string.IsNullOrWhiteSpace(product.Name))
    {
      return ResultType.Failure("Product is not valid. Product.Name is null or empty. Product.Name is required.");
    }

    if (quantity <= 0)
    {
      return ResultType.Failure("Quantity is less than or equal to 0. The quantity must be greater than zero.");
    }

    var order = new Order(
            CustomerName: customerName,
            Product: product,
            Quantity: quantity);

    return ResultType.Success(order);
  }
}
