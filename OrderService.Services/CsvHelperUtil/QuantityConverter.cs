using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace OrderService.Services.CsvHelperUtil;

public class QuantityConverter : DefaultTypeConverter
{
  public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
  {
    if (!Int32.TryParse(text, out int quantity))
    {
      quantity = -1;
    }
    return quantity;
  }
}