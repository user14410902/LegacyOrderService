using Microsoft.Extensions.Logging;
using OrderService.Common;
using OrderService.Data.Interfaces;
using OrderService.Services.AddOrders.Display;
using OrderService.Services.Sources;
using OrderService.UseCases.Implementations;

using ResultType = OrderService.Common.Result<bool, System.Collections.Generic.List<string>>;

namespace OrderService.Services.Services;

public class AddOrdersService : IOrdersCreationService
{
  private readonly ILogger<AddOrdersService> _logger;
  private readonly IProductRepository _productRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly ICreateOrderForCustomer _useCase;
  private readonly IAddOrderDisplay _display;

  public AddOrdersService(ILogger<AddOrdersService> logger,
  IProductRepository productRepository,
  IOrderRepository orderRepository,
  ICreateOrderForCustomer useCase,
  IAddOrderDisplay display)
  {
    _logger = logger;
    _productRepository = productRepository;
    _orderRepository = orderRepository;
    _useCase = useCase;
    _display = display;
  }

  public async Task<ResultType> ExecuteAsync(IRowsSource source, CancellationToken cancellationToken)
  {

    try
    {
      var records = source.Rows;
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
        if (row.Quantity > 0)
        {
          result = _useCase.Execute(row.CustomerName, product, row.Quantity);
        }
        else
        {
          result = Result<Entities.Order, string>.Failure($"Row {rowIndex}. Quantity must greater zero.");
        }

        if (result.IsSuccess)
        {
          _orderRepository.Add(result.Value);
          saveOrders = true;

          _display.DisplayOrder(result.Value);
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
        $"Error reading from {source.SourceDescription}. Error message: {e.Message}" });
    }
  }
}