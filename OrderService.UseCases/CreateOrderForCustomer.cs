using OrderService.Common;
using OrderService.Entities;

namespace OrderService.UseCases;

public class CreateOrderForCustomer
{

  public Result<Order> Execute(string customerName, Product product, int quantity)
  {
    if (string.IsNullOrWhiteSpace(customerName))
    {
      return Result<Order>.Failure("CustomerName is null or empty. CustomerName is required.");
    }

    if (product == null)
    {
      return Result<Order>.Failure("Product is null. Product is required.");
    }

    if (string.IsNullOrWhiteSpace(product.Name))
    {
      return Result<Order>.Failure("Product is not valid. Product.Name is null or empty. Product.Name is required.");
    }

    if (quantity <= 0)
    {
      return Result<Order>.Failure("Quantity is less than or equal to 0. The quantity must be greater than zero.");
    }


    var order = new Order(
            CustomerName: customerName,
            Product: product,
            Quantity: quantity);

    return Result<Order>.Success(order);
  }
}
