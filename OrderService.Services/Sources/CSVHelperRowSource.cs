using System.Globalization;
using CsvHelper.Configuration;
using OrderService.Services.CsvHelperUtil;

namespace OrderService.Services.Sources;

public class CSVHelperRowSource(string filePath) : IRowsSource
{

  public IEnumerable<OrderRow> Rows
  {
    get
    {
      var config = new CsvConfiguration(CultureInfo.InvariantCulture)
      {
        HasHeaderRecord = false,
        TrimOptions = TrimOptions.Trim
      };

      var reader = new StreamReader(filePath);
      var csv = new CsvHelper.CsvReader(reader, config);

      csv.Context.TypeConverterCache.AddConverter<int>(new QuantityConverter());
      csv.Context.RegisterClassMap<OrderRowMap>();

      return csv.GetRecords<OrderRow>().ToList(); //TODO Converting IEnumerable to a list which might be a performance issue.
    }
  }

  public string SourceDescription => filePath;

}