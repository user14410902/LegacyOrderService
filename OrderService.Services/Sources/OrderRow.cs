namespace OrderService.Services.Sources;

public class OrderRow
{
  public required string CustomerName { get; set; }
  public required string ProductName { get; set; }
  public int Quantity { get; set; }
}
