namespace OrderService.Services.AddOrderCSV;

public class CSVOrderRow
{
  public string CustomerName { get; set; } = string.Empty;
  public string ProductName { get; set; } = string.Empty;
  public string Quantity { get; set; } = string.Empty;
}