namespace OrderService.Services.AddOrderCSV;

public interface ICSVRowSource
{
  IEnumerable<CSVOrderRow> Rows { get; }

  string SourceDescription { get; }
}