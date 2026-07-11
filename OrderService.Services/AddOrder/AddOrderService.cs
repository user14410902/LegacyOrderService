using Microsoft.Extensions.Logging;
using OrderService.Common;
using OrderService.Data;
using OrderService.Data.Interfaces;
using OrderService.UseCases;

namespace OrderService.Services.AddOrder;

public class AddOrderService
{
  private readonly ILogger<AddOrderService> _logger;
  private readonly IProductRepository _productRepository;
  private readonly IOrderRepository _orderRepository;
  private readonly IAddOrderSources _sources;
  private readonly IAddOrderDisplay _display;

  public AddOrderService(ILogger<AddOrderService> logger,
  IProductRepository productRepository,
  IOrderRepository orderRepository,
  IAddOrderSources sources,
  IAddOrderDisplay display)
  {
    _logger = logger;
    _productRepository = productRepository;
    _orderRepository = orderRepository;
    _sources = sources;
    _display = display;
  }
  public async Task<Result<Guid>> ExecuteAsync(CancellationToken cancellationToken)
  {
    string? customerName = _sources.GetCustomerName();
    if (string.IsNullOrWhiteSpace(customerName))
    {
      var message = "Customer name is null. A valid customer name is required. Stopping.";
      _logger.LogError(message);
      return Result<Guid>.Failure(message);

    }

    string productName = _sources.GetProductName();
    var product = await _productRepository.GetProductAsync(productName, cancellationToken);
    if (product == null)
    {
      FormattableString message = $"Failed to retrieve {productName} from database. Stopping.";
      _logger.LogError(message.Format, message.GetArguments());
      return Result<Guid>.Failure(message.ToString());
    }

    int quantity = _sources.GetQuantity();

    _logger.LogInformation("Processing order...");

    var useCase = new CreateOrderForCustomer();
    var result = useCase.Execute(customerName, product, quantity);

    if (result.IsSuccess)
    {
      var order = result.Value;

      //first attempt to save the order to the database
      _logger.LogInformation("Saving order to database...");
      var newOrderId = await _orderRepository.SaveAsync(order, cancellationToken);

      //if the database save suceeds display the order
      _logger.LogInformation("Order complete!");
      _display.DisplayOrder(order);

      _logger.LogInformation("Done.");

      return Result<Guid>.Success(newOrderId);
    }
    else
    {
      var errorMessage = result.Error;
      FormattableString message = $"An error occured while creating the error. Error message: {errorMessage}";
      _logger.LogError(message.Format, message.GetArguments());
      return Result<Guid>.Failure(message.ToString());
    }


  }

}
