using System.Globalization;
using CsvHelper.Configuration;

namespace OrderService.Services.AddOrderCSV;

public class CSVHelperRowSource(string filePath) : ICSVRowSource
{

  public IEnumerable<CSVOrderRow> Rows
  {
    get
    {
      var config = new CsvConfiguration(CultureInfo.InvariantCulture)
      {
        HasHeaderRecord = false,
        TrimOptions = TrimOptions.Trim
      };

      var _reader = new StreamReader(filePath);
      var _csv = new CsvHelper.CsvReader(_reader, config);
      return _csv.GetRecords<CSVOrderRow>().ToList(); //TODO Converting IEnumerable to a list which might be a performance issue.
    }
  }

  public string SourceDescription => filePath;

}