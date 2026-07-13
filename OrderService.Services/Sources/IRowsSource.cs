namespace OrderService.Services.Sources;

public interface IRowsSource
{
  IEnumerable<OrderRow> Rows { get; }

  string SourceDescription { get; }
}