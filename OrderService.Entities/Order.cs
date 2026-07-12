namespace OrderService.Entities;

public record Order(string CustomerName,
Product Product,
int Quantity)
{

  public Guid Id { get; set; }
  public decimal Total { get; } = Quantity * Product.Price;

  public override string ToString()
  {
    return $"{CustomerName},{Product.Name},{Quantity},{Product.Price:F2},{Total:F2}";
  }
}

