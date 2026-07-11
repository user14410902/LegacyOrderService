using OrderService.Entities;

namespace OrderService.UseCases;

public class CreateOrderForCustomer
{

  public Order Execute(string customerName, Product product, int quantity)
  {
    if (string.IsNullOrWhiteSpace(customerName))
    {
      throw new ArgumentNullException(nameof(customerName), "CustomerName is null or empty. CustomerName is required.");
    }

    if (product == null)
    {
      throw new ArgumentNullException(nameof(product), "Product is null. Product is required.");
    }

    if (string.IsNullOrWhiteSpace(product.Name))
    {
      throw new ArgumentNullException(nameof(product.Name), "Product is not valid. Product.Name is null or empty. Product.Name is required.");
    }

    if (quantity <= 0)
    {
      throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity is less than or equal to 0. The quantity must be greater than zero.");
    }


    var order = new Order(
            CustomerName: customerName,
            Product: product,
            Quantity: quantity);

    return order;
  }
}
