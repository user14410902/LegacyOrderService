namespace OrderService.API.Responses.Orders.GetAllOrders;

public class OrderResponse(string CustomerName,
string ProductName,
int Quantity,
decimal Price,
decimal Total)
{
  public string CustomerName { get; } = CustomerName;
  public string ProductName { get; } = ProductName;
  public int Quantity { get; } = Quantity;
  public decimal Price { get; } = Price;
  public decimal Total { get; } = Total;
}