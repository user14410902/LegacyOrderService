namespace OrderService.Services.Sources;

public class ExternalRowSource(string customerName, string productName, int quantity) : IRowsSource
{
  private readonly string customerName = customerName;
  private readonly string productName = productName;
  private readonly int quantity = quantity;
  public IEnumerable<OrderRow> Rows
  {
    get
    {
      return [new OrderRow {
        CustomerName = customerName,
        ProductName = productName,
        Quantity = quantity }];

    }
  }

  public string SourceDescription { get; set; } = "External";


}