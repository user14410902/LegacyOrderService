using System.Globalization;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using OrderService.Common;
using OrderService.Data.Interfaces;
using OrderService.UseCases;
using OrderService.UseCases.Implementations;

using ResultType = OrderService.Common.Result<bool, System.Collections.Generic.List<string>>;

namespace OrderService.Services.AddOrderCSV;

public class AddOrderCSVService : IService<bool, List<string>>
{
  private readonly ILogger<AddOrderCSVService> _logger;
  private readonly IProductRepository _productRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly ICSVRowSource _source;
  private readonly ICreateOrderForCustomer _useCase;

  public AddOrderCSVService(ILogger<AddOrderCSVService> logger,
  IProductRepository productRepository,
  IOrderRepository orderRepository,
  ICSVRowSource source,
  ICreateOrderForCustomer useCase)
  {
    _logger = logger;
    _productRepository = productRepository;
    _orderRepository = orderRepository;
    _source = source;
    _useCase = useCase;
  }

  public async Task<ResultType> ExecuteAsync(CancellationToken cancellationToken)
  {

    try
    {
      var records = _source.Rows;
      if (records.Count() == 0)
      {
        return ResultType.Failure(new List<string> { "No records to process." });
      }

      var errorList = new List<string>();
      var rowIndex = 0;
      bool saveOrders = false;


      foreach (var row in records)
      {
        var product = await _productRepository.GetProductAsync(row.ProductName, cancellationToken);
        if (product == null)
        {
          errorList.Add($"Row {rowIndex}. Product {row.ProductName} not found");
          continue;
        }

        Result<Entities.Order, string> result;
        if (int.TryParse(row.Quantity, out int quantity))
        {
          result = _useCase.Execute(row.CustomerName, product, quantity);
        }
        else
        {
          result = Result<Entities.Order, string>.Failure($"Failed to parse quantity: {row.Quantity}");
        }

        if (result.IsSuccess)
        {
          _orderRepository.Add(result.Value);
          saveOrders = true;
        }
        else
        {
          errorList.Add($"Row {rowIndex}. Failed to process row. Error message: {result.Error}");
        }

        rowIndex++;
      }
      if (saveOrders)
      {
        await _orderRepository.SaveAsync(cancellationToken);
      }

      if (errorList.Count == 0)
      {
        return ResultType.Success(true);
      }
      else
      {
        if (saveOrders)
        {
          var preMessage = string.Empty;
          if (errorList.Count == records.Count())
          {
            preMessage = $"All {records.Count()} rows not processed.";
          }
          else
          {
            preMessage = $"Input partially processed. {errorList.Count} of {records.Count()} row(s) not processed.";
          }
          errorList = errorList.Prepend(preMessage).ToList();
        }
        return ResultType.Failure(errorList);

      }
    }
    catch (System.IO.FileNotFoundException e)
    {
      return ResultType.Failure(new List<String> {
        $"File not found {_source.SourceDescription}. Error message: {e.Message}" });
    }
  }
}