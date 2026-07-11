namespace OrderService.Entities;

public record Order(string CustomerName,
Product Product,
int Quantity)
{

  public Guid Id { get; set; }
  public double Total { get; } = Quantity * Product.Price;

}

