namespace OrderService.API.Requests.Orders.PostOrder;

public class PostOrderRequest
{
  public required string CustomerName { get; set; }
  public required string ProductName { get; set; }
  public required int Quantity { get; set; }
}