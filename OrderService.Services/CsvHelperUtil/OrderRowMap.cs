using CsvHelper.Configuration;
using OrderService.Services.Sources;

namespace OrderService.Services.CsvHelperUtil;

public class OrderRowMap : ClassMap<OrderRow>
{
  public OrderRowMap()
  {
    Map(o => o.CustomerName);
    Map(o => o.ProductName);
    Map(o => o.Quantity).TypeConverter<QuantityConverter>();
  }
}